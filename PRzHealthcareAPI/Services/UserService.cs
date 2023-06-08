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
using System.Security.Principal;
using System.Text;

namespace PRzHealthcareAPI.Services
{
    public interface IUserService
    {
        void ChangePassword(ChangeUserPasswordDto dto);
        LoginUserDto? GenerateToken(LoginUserDto dto);
        void Register(RegisterUserDto dto);
        void ConfirmMail(string hashcode);
        List<UserDto> GetDoctorsList();
        void ResetPassword(ResetUserPasswordDto dto);
        void ResetPasswordRequest(string email);
        void ResetPasswordCheckHashCode(string hashcode);
        List<UserDto> GetPatientsList();
        UserDto GetSelectedUser(int userId);
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

        /// <summary>
        /// Rejestracja użytkownika w systemie
        /// </summary>
        /// <param name="dto">Obiekt użytkownika</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public void Register(RegisterUserDto dto)
        {
            var loginExists = _dbContext.Accounts.Any(x => x.Acc_Login == dto.Login || x.Acc_Email == dto.Email || x.Acc_Pesel == dto.Pesel);
            if (loginExists)
            {
                throw new BadRequestException("Konto o tym loginie, adresie e-mail lub peselu już istnieje.");
            }

            try
            {
                var newUser = _mapper.Map<Account>(dto);
                newUser.Acc_RegistrationHash = Tools.CreateRandomToken(64);
                newUser.Acc_InsertedDate = DateTime.Now;
                newUser.Acc_ModifiedDate = DateTime.Now;
                newUser.Acc_IsActive = true;
                newUser.Acc_PhotoId = null;
                newUser.Acc_Password = _passwordHasher.HashPassword(newUser, dto.Password);
                newUser.Acc_AtyId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Niepotwierdzony").Aty_Id;
                _dbContext.Accounts.Add(newUser);
                _dbContext.SaveChanges();

                Tools.SendRegistrationMail(_emailSettings, newUser, _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Rejestracja użytkownika w systemie PRz Healthcare"));
                Notification notif = new()
                {
                    Not_EveId = _dbContext.Events.FirstOrDefault().Eve_Id,
                    Not_NtyId = _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Rejestracja użytkownika w systemie PRz Healthcare").Nty_Id,
                    Not_SendTime = DateTime.Now,
                    Not_IsActive = true,
                    Not_InsertedDate = DateTime.Now,
                    Not_InsertedAccId = newUser.Acc_Id,
                    Not_ModifiedDate = DateTime.Now,
                    Not_ModifiedAccId = newUser.Acc_Id,
                };
                _dbContext.Notifications.Add(notif);
                _dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }

        /// <summary>
        /// Logowanie użytkownika - generowanie tokenu
        /// </summary>
        /// <param name="dto">Obiekt użytkownika</param>
        /// <returns>Obiekt użytkownika z tokenem</returns>
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
            if (user.AccountType.Aty_Name == "Niepotwierdzony")
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

            LoginUserDto loginUser = new()
            {
                AccId = user.Acc_Id,
                Login = dto.Login,
                Name = user.Acc_Firstname,
                AtyId = user.Acc_AtyId,
                Token = tokenHandler.WriteToken(token)
            };

            return loginUser;
        }

        /// <summary>
        /// Pobranie listy doktorów
        /// </summary>
        /// <returns>Lista doktorów</returns>
        public List<UserDto> GetDoctorsList()
        {
            var doctorAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Doktor").Aty_Id;
            var list = _dbContext.Accounts.Where(x => x.Acc_AtyId == doctorAccountTypeId && x.Acc_IsActive).ToList();
            if(list is null)
            {
                return new List<UserDto>();
            }

            List<UserDto> listUserDto = new();

            foreach (var account in list)
            {
                listUserDto.Add(_mapper.Map<UserDto>(account));
            }

            return listUserDto;
        }

        /// <summary>
        /// Pobranie szczegółów wybranego użytkownika
        /// </summary>
        /// <param name="userId">Id wybranego użytkownika</param>
        /// <returns>Szczegóły użytkownika</returns>
        /// <exception cref="NotFoundException"></exception>
        public UserDto GetSelectedUser(int userId)
        {
            var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Id == userId);
            if (user == null)
            {
                throw new NotFoundException("Użytkownik nie istnieje.");
            }

            var userDto = _mapper.Map<UserDto>(user);


            return userDto;
        }

        /// <summary>
        /// Pobranie listy pacjentów
        /// </summary>
        /// <returns>Lista obiektów pacjentów</returns>
        public List<UserDto> GetPatientsList()
        {
            var patientAccountTypeId = _dbContext.AccountTypes.FirstOrDefault(x => x.Aty_Name == "Pacjent").Aty_Id;
            var list = _dbContext.Accounts.Where(x => x.Acc_AtyId == patientAccountTypeId && x.Acc_IsActive).ToList();
            if (list is null)
            {
                return new List<UserDto>();
            }

            List<UserDto> listUserDto = new();

            foreach (var account in list)
            {
                listUserDto.Add(_mapper.Map<UserDto>(account));
            }

            return listUserDto;
        }

        /// <summary>
        /// Potwierdzenie konta poprzez e-mail
        /// </summary>
        /// <param name="hashcode">Kod otrzymany w wiadomości email</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public void ConfirmMail(string hashcode)
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
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Wystąpił błąd podczas próby potwierdzenia użytkownika.");
            }
        }

        /// <summary>
        /// Zmiana hasła użytkownika
        /// </summary>
        /// <param name="dto">Obiekt zmiany hasła użytkownika</param>
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

        /// <summary>
        /// Wysłanie powiadomienia o próbie zmiany hasła
        /// </summary>
        /// <param name="email">Adres e-mail użytkownika</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public void ResetPasswordRequest(string email)
        {
            try
            {
                var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Email == email);

                if (user != null)
                {
                    user.Acc_ReminderHash = Tools.CreateRandomToken(64);
                    user.Acc_ReminderExpire = DateTime.Now.AddDays(1);
                    _dbContext.SaveChanges();

                    Tools.SendPasswordReminder(_emailSettings, user, _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Przypomnienie hasła w systemie PRz Healthcare"));
                    Notification notif = new()
                    {
                        Not_EveId = _dbContext.Events.FirstOrDefault().Eve_Id,
                        Not_NtyId = _dbContext.NotificationTypes.FirstOrDefault(x => x.Nty_Name == "Przypomnienie hasła w systemie PRz Healthcare").Nty_Id,
                        Not_SendTime = DateTime.Now,
                        Not_IsActive = true,
                        Not_InsertedDate = DateTime.Now,
                        Not_InsertedAccId = user.Acc_Id,
                        Not_ModifiedDate = DateTime.Now,
                        Not_ModifiedAccId = user.Acc_Id,
                    };
                    _dbContext.Notifications.Add(notif);
                    _dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new BadRequestException($@"Wystąpił błąd podczas próby zresetowania hasła. Spróbuj ponownie za moment. ({ex.Message})");
            }
            
        }

        /// <summary>
        /// Zmiana hasła użytkownika
        /// </summary>
        /// <param name="dto">Obiekt zmiany hasła użytkownika</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public void ResetPassword(ResetUserPasswordDto dto)
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
               
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Wystąpił błąd podczas próby zresetowania hasła. Spróbuj ponownie za moment.");
            }

        }

        /// <summary>
        /// Weryfikacja kodu Hash użytkownika przez zmianą hasła
        /// </summary>
        /// <param name="hashcode">Kod zmiany hasła użytkownika</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public void ResetPasswordCheckHashCode(string hashcode)
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

            }
            catch (Exception ex)
            {
                throw new BadRequestException("Wystąpił błąd podczas próby potwierdzenia użytkownika na podstawie adres e-mail.");
            }
        }
        
        
    }
}
