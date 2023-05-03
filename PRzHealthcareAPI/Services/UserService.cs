using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PRzHealthcareAPI.Exceptions;
using PRzHealthcareAPI.Helpers;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Models.DTO;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PRzHealthcareAPI.Services
{
    public interface IUserService
    {
        void ChangePassword(ChangeUserPasswordDto dto);
        LoginUserDto? GenerateToken(LoginUserDto dto);
        Task<string> Register(RegisterUserDto dto);
        Task<string> ConfirmMail(string hashcode);
        List<UserDto> GetDoctorsList();
        Task<string> ResetPassword(ResetUserPasswordDto dto);
        Task<string> ResetPasswordRequest(string email);
        Task<string> ResetPasswordCheckHashCode(string hashcode);
        List<UserDto> GetPatientsList();
    }

    public class UserService : IUserService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly AuthenticationSettings _authentication;
        private readonly EmailSettings _emailSettings;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(HealthcareDbContext dbContext, AuthenticationSettings authentication, EmailSettings emailSettings, IPasswordHasher<Account> passwordHasher, IMapper mapper)
        {
            this._dbContext = dbContext;
            this._authentication = authentication;
            this._emailSettings = emailSettings;
            this._passwordHasher = passwordHasher;
            this._mapper = mapper;
        }

        public async Task<string> Register(RegisterUserDto dto)
        {
            var loginExists = _dbContext.Accounts.Any(x => x.Acc_Login == dto.Login || x.Acc_Email == dto.Email || x.Acc_Pesel == dto.Pesel);
            if (loginExists)
            {
                throw new BadRequestException("Konto o tym loginie, adresie e-mail lub peselu już istnieje.");
            }

            try
            {
                var newUser = _mapper.Map<Account>(dto);
                newUser.Acc_RegistrationHash = CreateRandomToken();
                newUser.Acc_InsertedDate = DateTime.Now;
                newUser.Acc_ModifiedDate = DateTime.Now;
                newUser.Acc_IsActive = true;
                newUser.Acc_PhotoId = null;
                newUser.Acc_Password = _passwordHasher.HashPassword(newUser, dto.Password);
                newUser.Acc_AtyId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Niepotwierdzony").Aty_Id;
                _dbContext.Accounts.Add(newUser);

                await _dbContext.SaveChangesAsync();

                Tools.SendRegistrationMail(_emailSettings, newUser, _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Rejestracja użytkownika w systemie PRz Healthcare"));
                return "";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }

        public LoginUserDto? GenerateToken(LoginUserDto dto)
        {
            var user = _dbContext.Accounts
                .Include(r => r.AccountType)
                .FirstOrDefault(x => x.Acc_Login == dto.Login);

            if (user is null)
            {
                throw new BadRequestException("Błędny login lub hasło.");
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.Acc_Password, dto.Password) == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Błędny login lub hasło.");
            }

            /*  Niezarejestrowany   */
            if (user.AccountType.Aty_Name == "Niezarejestrowany")
            {
                throw new BadRequestException("Twoje konto nie zostało wciąż potwierdzone.");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Acc_Login.ToString()),
                new Claim(ClaimTypes.Name, $@"{user.Acc_Firstname} {user.Acc_Lastname}"),
                new Claim(ClaimTypes.Role, user.AccountType.Aty_Name.ToString()),
                new Claim(ClaimTypes.SerialNumber, user.Acc_Id.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authentication.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(_authentication.JwtExpireHours);

            var token = new JwtSecurityToken(_authentication.JwtIssuer, _authentication.JwtIssuer, claims, expires: expires, signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();

            LoginUserDto loginUser = new LoginUserDto()
            {
                AccId = user.Acc_Id,
                Login = dto.Login,
                Name = user.Acc_Firstname,
                AtyId = user.Acc_AtyId,
                Token = tokenHandler.WriteToken(token)
            };

            return loginUser;
        }

        public List<UserDto> GetDoctorsList()
        {
            var doctorAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Doktor").Aty_Id;
            var list = _dbContext.Accounts.Where(x => x.Acc_AtyId == doctorAccountTypeId && x.Acc_IsActive).ToList();
            if(list is null)
            {
                return new List<UserDto>();
            }

            List<UserDto> listUserDto = new List<UserDto>();

            foreach (var account in list)
            {
                listUserDto.Add(_mapper.Map<UserDto>(account));
            }

            return listUserDto;
        }
        public List<UserDto> GetPatientsList()
        {
            var patientAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Pacjent").Aty_Id;
            var list = _dbContext.Accounts.Where(x => x.Acc_AtyId == patientAccountTypeId && x.Acc_IsActive).ToList();
            if (list is null)
            {
                return new List<UserDto>();
            }

            List<UserDto> listUserDto = new List<UserDto>();

            foreach (var account in list)
            {
                listUserDto.Add(_mapper.Map<UserDto>(account));
            }

            return listUserDto;
        }
        public async Task<string> ConfirmMail(string hashcode)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(hashcode))
                {
                    throw new NotFoundException("Błędny kod.");
                }

                var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_RegistrationHash == hashcode);
                if (user == null)
                {
                    throw new NotFoundException("Błędny kod.");
                }

                var patientAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Pacjent").Aty_Id;

                user.Acc_AtyId = patientAccountTypeId;
                await _dbContext.SaveChangesAsync();
                return "Ok";
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Wystąpił błąd podczas próby potwierdzenia użytkownika.");
            }
        }
        public void ChangePassword(ChangeUserPasswordDto dto)
        {
            var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Login == dto.Login);

            if (user is null)
            {
                throw new NotFoundException("Użytkownik nie istnieje.");
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.Acc_Password, dto.OldPassword) == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Podane hasło jest nieprawidłowe.");
            }

            if (dto.NewPassword != dto.NewPasswordRepeat)
            {
                throw new BadRequestException("Hasła są niezgodne.");
            }

            user.Acc_Password = _passwordHasher.HashPassword(user, dto.NewPassword);

            _dbContext.Accounts.Update(user);
            _dbContext.SaveChanges();
        }

        public async Task<string> ResetPasswordRequest(string email)
        {
            try
            {
                var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Email == email);

                if (user != null)
                {
                    user.Acc_ReminderHash = CreateRandomToken();
                    user.Acc_ReminderExpire = DateTime.Now.AddDays(1);
                    _dbContext.SaveChanges();

                    Tools.SendPasswordReminder(_emailSettings, user, _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Przypomnienie hasła w systemie PRz Healthcare"));
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Wystąpił błąd podczas próby zresetowania hasła. Spróbuj ponownie za moment.");
            }
            
        }
        public async Task<string> ResetPassword(ResetUserPasswordDto dto)
        {
            try
            {
                var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_ReminderHash == dto.HashCode);

                if (user != null)
                {
                    if (dto.NewPassword != dto.NewPasswordRepeat)
                    {
                        throw new BadRequestException("Hasła są niezgodne.");
                    }

                    user.Acc_Password = _passwordHasher.HashPassword(user, dto.NewPassword);

                    _dbContext.Accounts.Update(user);
                    _dbContext.SaveChanges();
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Wystąpił błąd podczas próby zresetowania hasła. Spróbuj ponownie za moment.");
            }

        }
        public async Task<string> ResetPasswordCheckHashCode(string hashcode)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(hashcode))
                {
                    throw new NotFoundException("Błędny kod.");
                }

                var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_ReminderHash == hashcode);
                if (user == null)
                {
                    throw new NotFoundException("Błędny kod.");
                }

                if(user.Acc_ReminderExpire > DateTime.Now)
                {
                    throw new BadRequestException("Link został przedawniony. Wybierz opcję 'Przypomnij hasło' ponownie");
                }

                return "Ok";
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Wystąpił błąd podczas próby potwierdzenia użytkownika na podstawie adres e-mail.");
            }
        }
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        
    }
}
