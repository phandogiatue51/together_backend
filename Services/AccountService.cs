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
        private readonly StaffRepo _staffRepo;
        private readonly CloudinaryService _imageStorageService;

        public AccountService(AccountRepo accountRepo, PasswordHelper passwordHelper, IConfiguration configuration, 
            StaffRepo staffRepo, CloudinaryService imageStorageService)
        {
            _accountRepo = accountRepo;
            _passwordHelper = passwordHelper;
            _configuration = configuration;
            _staffRepo = staffRepo;
            _imageStorageService = imageStorageService;
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

        public async Task<(bool Success, string Message, int AccountId)> CreateAccount(CreateUserDto dto, AccountRole role, IFormFile? imageFile = null)
        {
            var existingAccount = await _accountRepo.ExistsAsync(a => a.Email == dto.Email);
            if (existingAccount)
            {
                return (false, "Email này đã được tạo cho một tài khoản khác!", 0);
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

            if (imageFile != null && imageFile.Length > 0)
            {
                account.ProfileImageUrl = await _imageStorageService.UploadImageAsync(imageFile);
            }

            await _accountRepo.AddAsync(account);
            return (true, "Tạo tài khoản thành công!", account.Id);
        }

        public async Task<(bool Success, string Message)> UpdateAccount(int id, UpdateUserDto dto, IFormFile? imageFile)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return (false, "Không tìm thấy tài khoản!");
            }

            account.Name = dto.Name ?? account.Name;
            account.PhoneNumber = dto.PhoneNumber ?? account.PhoneNumber;
            account.DateOfBirth = dto.DateOfBirth ?? account.DateOfBirth;
            account.IsFemale = dto.IsFemale ?? account.IsFemale;
            account.Bio = dto.Bio ?? account.Bio;

            if (!string.IsNullOrEmpty(dto.Email) && account.Email != dto.Email)
            {
                var emailExists = await _accountRepo.ExistsAsync(a =>
                    a.Email == dto.Email && a.Id != id);

                if (emailExists)
                    return (false, "Email này đã được tạo cho một tài khoản khác!");

                account.Email = dto.Email;
            }

            account.UpdatedAt = DateTime.UtcNow;

            if (imageFile != null && imageFile.Length > 0)
            {
                account.ProfileImageUrl = await _imageStorageService.UpdateImageAsync(
                    account.ProfileImageUrl, imageFile);
            }

            await _accountRepo.UpdateAsync(account);
            return (true, "Cập nhật tài khoản thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteAccount(int id)
        {
            var account = await _accountRepo.GetByIdAsync(id);
            if (account == null)
            {
                return (false, "Không tìm thấy tài khoản!");
            }

            await _accountRepo.DeleteAsync(account);
            return (true, "Xóa tài khoản thành công!");
        }

        public async Task<(bool Success, string Message, string Token, AccountRole Role)> LoginAsync(LoginDto dto)
        {
            var user = await _accountRepo.GetByEmailAsync(dto.Email);

            if (user == null)
                return (false, "Không tìm thấy tài khoản!", null, default(AccountRole));

            if (user.Status == AccountStatus.Inactive)
                return (false, "Tài khoản đã dừng hoạt động! Nếu bạn có thắc mắc, hãy liên hệ Admin.", null, default(AccountRole));

            if (!_passwordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return (false, "Sai mật khẩu!", null, default(AccountRole));

            var claims = await GenerateUserClaims(user);

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

            return (true, "Đăng nhập thành công!", tokenString, user.Role);
        }

        private async Task<List<Claim>> GenerateUserClaims(Account user)
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

            if (user.Role == AccountRole.Staff)
            {
                var staff = await _staffRepo.GetByAccountId(user.Id);

                if (staff != null)
                {
                    claims.Add(new Claim("StaffId", staff.Id.ToString()));
                    claims.Add(new Claim("OrganizationId", staff.OrganizationId.ToString()));
                    claims.Add(new Claim("StaffRole", staff.Role.ToString()));
                }
            }

            return claims;
        }

        public async Task<(bool Success, string Message)> ChangePassword(int accountId, ChangePasswordDto dto)
        {
            var account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null)
            {
                return (false, "Không tìm thấy tài khoản!");
            }
            if (!_passwordHelper.VerifyPassword(dto.CurrentPassword, account.PasswordHash))
            {
                return (false, "Sai mật khẩu!");
            }
            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                return (false, "Mật khẩu mới và mật khẩu xác nhận không trùng khớp");
            }
            account.PasswordHash = _passwordHelper.HashPassword(dto.NewPassword);
            await _accountRepo.UpdateAsync(account);
            return (true, "Thay đổi mật khẩu thành công!");
        }

        public async Task<List<ViewUserDto>> GetAccountsByFilter(UserFilterDto filter)
        {
            var accounts = await _accountRepo.GetByFilterAsync(filter);
            return accounts.Select(MapToViewUserDto).ToList();
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
                Bio = account.Bio,
                Hour = account.Hour,
                ProfileImageUrl = account.ProfileImageUrl,
                Role = (AccountRole)account.Role,
                RoleName = account.Role.ToFriendlyName(),
                Status = (AccountStatus)account.Status,
                StatusName = account.Status.ToFriendlyName(),
                CreatedAt = account.CreatedAt != null ? DateOnly.FromDateTime(account.CreatedAt) : null
            };
        }
    }
}