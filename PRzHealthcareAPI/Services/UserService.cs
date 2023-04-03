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
    }

    public class UserService : IUserService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly AuthenticationSettings _authentication;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(HealthcareDbContext dbContext, AuthenticationSettings authentication, IPasswordHasher<Account> passwordHasher, IMapper mapper)
        {
            this._dbContext = dbContext;
            this._authentication = authentication;
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
                var newPhoto = new BinData()
                {
                    Bin_Name = $@"{dto.Login}_Photo",
                    Bin_Data = dto.PhotoBinary is null? "" : dto.PhotoBinary,
                    Bin_InsertedAccId = 0,
                    Bin_InsertedDate = DateTime.Now,
                    Bin_ModifiedAccId = 0,
                    Bin_ModifiedDate = DateTime.Now,
                };

                _dbContext.BinData.Add(newPhoto);

                var newUser = _mapper.Map<Account>(dto);
                newUser.Acc_RegistrationHash = CreateRandomToken();
                newUser.Acc_InsertedDate = DateTime.Now;
                newUser.Acc_ModifiedDate = DateTime.Now;
                newUser.Acc_IsActive = true;
                newUser.Acc_PhotoId = newPhoto.Bin_Id;
                newUser.Acc_Password = _passwordHasher.HashPassword(newUser, dto.Password);
                newUser.Acc_AtyId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Niezarejestrowany").Aty_Id;
                _dbContext.Accounts.Add(newUser);

                await _dbContext.SaveChangesAsync();

                Tools.SendRegistrationMail(newUser, _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Rejestracja użytkownika w systemie PRz Healthcare"));
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
            var list = _dbContext.Accounts.Where(x => x.Acc_AtyId == 3).ToList();
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

                user.Acc_AtyId = 1;
                await _dbContext.SaveChangesAsync();
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
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
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}
