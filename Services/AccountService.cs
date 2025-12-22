using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Together.DTOs.User;
using Together.Helpers;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class AccountService
    {
        private readonly AccountRepo _accountRepo;
        private readonly PasswordHelper _passwordHelper;
        private readonly IConfiguration _configuration;

        public AccountService(AccountRepo accountRepo, PasswordHelper passwordHelper, IConfiguration configuration)
        {
            _accountRepo = accountRepo;
            _passwordHelper = passwordHelper;
            _configuration = configuration;
        }

        public async Task<List<ViewUserDto>> GetAllAccounts()
        {
            var accounts = await _accountRepo.GettAll();

            return accounts.Select(MapToViewUserDto).ToList();
        }

        public async Task<ViewUserDto?> GetAccountById(int id)
        {
            var account = await _accountRepo.GetByIdAsync(id);

            if (account == null)
                return null;

            return MapToViewUserDto(account);
        }

        public async Task<(bool Success, string Message, int AccountId)> CreateAccount(CreateUserDto dto, AccountRole role)
        {
            var existingAccount = await _accountRepo.ExistsAsync(a => a.Email == dto.Email);
            if (existingAccount)
            {
                return (false, "Account with this email already exists.", 0);
            }

            var account = new Account
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                IsFemale = dto.IsFemale,
                PasswordHash = _passwordHelper.HashPassword(dto.Password),
                Role = role,
                Status = AccountStatus.Active
            };

            await _accountRepo.AddAsync(account);
            return (true, "Account created successfully!", account.Id);
        }

        public async Task<(bool Success, string Message)> UpdateAccount(int id, UpdateUserDto dto)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return (false, "Account not found.");
            }

            account.Name = dto.Name ?? account.Name;
            account.PhoneNumber = dto.PhoneNumber ?? account.PhoneNumber;
            account.DateOfBirth = dto.DateOfBirth ?? account.DateOfBirth;
            account.IsFemale = dto.IsFemale ?? account.IsFemale;

            if (!string.IsNullOrEmpty(dto.Email) && account.Email != dto.Email)
            {
                var emailExists = await _accountRepo.ExistsAsync(a =>
                    a.Email == dto.Email && a.Id != id);

                if (emailExists)
                    return (false, "Email already in use by another account.");

                account.Email = dto.Email;
            }

            account.UpdatedAt = DateTime.UtcNow; 

            await _accountRepo.UpdateAsync(account);
            return (true, "Account updated successfully!");
        }

        public async Task<(bool Success, string Message)> DeleteAccount(int id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return (false, "Account not found.");
            }

            await _accountRepo.DeleteAsync(account);
            return (true, "Account deleted successfully!");
        }

        public async Task<(bool Success, string Message, string Token, AccountRole Role)> LoginAsync(LoginDto dto)
        {
            var user = await _accountRepo.GetByEmailAsync(dto.Email);

            if (user == null)
                return (false, "Account does not exist!", null, default(AccountRole));

            if (user.Status == AccountStatus.Inactive)
                return (false, "Account is inactive. Please contact support.", null, default(AccountRole));

            if (user.Status == AccountStatus.Banned)
                return (false, "Account is banned. Please contact support.", null, default(AccountRole));

            if (!_passwordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return (false, "Wrong password", null, default(AccountRole));

            var claims = GenerateUserClaims(user);

            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
                throw new InvalidOperationException("JWT Key not configured");

            var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length < 32)
            {
                Array.Resize(ref keyBytes, 32);
            }

            var securityKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "TogetherAPI",
                audience: _configuration["Jwt:Audience"] ?? "TogetherClient",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (true, "Login successful", tokenString, user.Role);
        }

        private List<Claim> GenerateUserClaims(Account user)
        {
            var claims = new List<Claim>
            {
                new Claim("AccountId", user.Id.ToString() ?? "0"),
                new Claim("Email", user.Email ?? ""),
                new Claim("Role", user.Role.ToString() ?? "User"),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? "0"),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role.ToString() ?? "User"),
            };
            return claims;
        }

        public async Task<(bool Success, string Message)> ChangePassword(int accountId, ChangePasswordDto dto)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null)
            {
                return (false, "Account not found!");
            }
            if (!_passwordHelper.VerifyPassword(dto.CurrentPassword, account.PasswordHash))
            {
                return (false, "Current password is incorrect.");
            }
            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                return (false, "New password and confirmation do not match.");
            }
            account.PasswordHash = _passwordHelper.HashPassword(dto.NewPassword);
            await _accountRepo.UpdateAsync(account);
            return (true, "Password changed successfully!");
        }

        public ViewUserDto MapToViewUserDto (Account account)
        {
            return new ViewUserDto
            {
                Id = (int)account.Id,
                Name = account.Name,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber ?? string.Empty,
                DateOfBirth = account.DateOfBirth ?? default(DateOnly),
                IsFemale = account.IsFemale ?? false,
                Role = (AccountRole)account.Role,
                Status = (AccountStatus)account.Status
            };
        }
    }
}