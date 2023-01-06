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
using System.Text;

namespace PRzHealthcareAPI.Services
{
    public interface IUserService
    {
        void ChangePassword(LoginUserDto dto);
        LoginUserDto? GenerateToken(LoginUserDto dto);
        void Register(RegisterUserDto dto);
    }

    public class UserService : IUserService
    {
        private readonly HealthcareDbContext _dbContext;
        private readonly AuthenticationSettings _authentication;
        private readonly IPasswordHasher<Account> _passwordHasher;

        public UserService(HealthcareDbContext dbContext, AuthenticationSettings authentication, IPasswordHasher<Account> passwordHasher)
        {
            this._dbContext = dbContext;
            this._authentication = authentication;
            this._passwordHasher = passwordHasher;
        }

        public void Register(RegisterUserDto dto)
        {
            var newUser = new Account()
            {
                Acc_Id = dto.Id,
                Acc_AtyId = dto.AtyId,
                Acc_Login = dto.Login,
                Acc_Firstname = dto.Firstname,
                Acc_Secondname = dto.Secondname,
                Acc_Lastname = dto.Lastname,
                Acc_DateOfBirth = dto.DateOfBirth,
                Acc_Pesel = dto.Pesel,
                Acc_Email = dto.Email,
                Acc_ContactNumber = dto.ContactNumber,
                Acc_IsActive = true,
                Acc_InsertedDate = DateTime.Now,
                Acc_ModifiedDate = DateTime.Now,
            };
            var newPhoto = new BinData()
            {
                Bin_Name = $@"{newUser.Acc_Login}_Photo",
                Bin_Data = dto.PhotoBinary,
                Bin_InsertedAccId = 0,
                Bin_InsertedDate = DateTime.Now,
                Bin_ModifiedAccId = 0,
                Bin_ModifiedDate = DateTime.Now,

            };

            _dbContext.BinData.Add(newPhoto);
            _dbContext.SaveChanges();

            newUser.Acc_PhotoId = newPhoto.Bin_Id;
            newUser.Acc_Password = _passwordHasher.HashPassword(newUser, dto.Password);
            _dbContext.Accounts.Add(newUser);
            _dbContext.SaveChanges();
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

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Acc_Login.ToString()),
                new Claim(ClaimTypes.Name, $@"{user.Acc_Firstname} {user.Acc_Lastname}"),
                new Claim(ClaimTypes.Role, user.AccountType.Aty_Name.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authentication.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(_authentication.JwtExpireHours);

            var token = new JwtSecurityToken(_authentication.JwtIssuer, _authentication.JwtIssuer, claims, expires: expires, signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();

            LoginUserDto loginUser = new LoginUserDto()
            {
                Login = dto.Login,
                Name = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Login == dto.Login).Acc_Firstname,
                AtyId = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Login == dto.Login).Acc_AtyId,
                Token = tokenHandler.WriteToken(token)
            };

            return loginUser;
        }
        public void ChangePassword(LoginUserDto dto)
        {
            var user = _dbContext.Accounts.FirstOrDefault(x => x.Acc_Login == dto.Login);

            if (user is null)
            {
                throw new NotFoundException("Użytkownik nie istnieje.");
            }

            user.Acc_Password = _passwordHasher.HashPassword(user, dto.Password);

            _dbContext.Accounts.Update(user);
            _dbContext.SaveChanges();
        }

    }
}
