using Together.DTOs.Organ;
using Together.DTOs.Staf;
using Together.Helpers;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class StaffService
    {
        private readonly StaffRepo _staffRepo;
        private readonly AccountService _accountService;
        private readonly OrganRepo _organRepo;
        private readonly AccountRepo _accountRepo;
        private readonly PasswordHelper _passwordHelper;

        public StaffService(StaffRepo staffRepo, AccountService accountService, OrganRepo organRepo,
            AccountRepo accountRepo, PasswordHelper passwordHelper)
        {
            _staffRepo = staffRepo;
            _accountService = accountService;
            _organRepo = organRepo;
            _accountRepo = accountRepo;
            _passwordHelper = passwordHelper;
        }

        public async Task<List<ViewStaffDto>> GetAllStaff()
        {
            var staffs = await _staffRepo.GettAll();
            return staffs.Select(m => new ViewStaffDto
            {
                Id = m.Id,
                OrganizationId = m.OrganizationId,
                OrganizationName = m.Organization?.Name,
                AccountId = m.AccountId,
                Name = m.StaffAccount.Name,
                Email = m.StaffAccount.Email,
                StaffRole = m.Role.ToString(),
                JoinedDate = m.JoinedDate
            }).ToList();
        }

        public async Task<ViewStaffDto?> GetStaffById(int id)
        {
            var staff = await _staffRepo.GetByIdAsync(id);
            return new ViewStaffDto
            {
                Id = staff.Id,
                OrganizationId = staff.OrganizationId,
                OrganizationName = staff.Organization?.Name,
                AccountId = staff.AccountId,
                Name = staff.StaffAccount.Name,
                Email = staff.StaffAccount.Email,
                StaffRole = staff.Role.ToString(),
                JoinedDate = staff.JoinedDate
            };
        }

        public async Task<List<ViewStaffDto>> GetStaffByOrganId(int organId)
        {
            var staffs = await _staffRepo.GetStaffByOrganId(organId);
            return staffs.Select(m => new ViewStaffDto
            {
                Id = m.Id,
                OrganizationId = m.OrganizationId,
                AccountId = m.AccountId,
                Name = m.StaffAccount.Name,
                Email = m.StaffAccount.Email,
                StaffRole = m.Role.ToString(),
                JoinedDate = m.JoinedDate
            }).ToList();
        }

        public async Task<(bool Success, string Message, int? StaffId)> CreateStaff(CreateStaffDto dto)
        {
            try
            {
                var organization = await _organRepo.GetByIdAsync(dto.OrganizationId);
                if (organization == null)
                    return (false, "Organization not found", null);

                int accountId;

                var accountResult = await _accountService.CreateAccount(dto.NewAccount, AccountRole.Staff);

                if (!accountResult.Success)
                    return (false, $"Failed to create account: {accountResult.Message}", null);

                accountId = accountResult.AccountId;

                var staff = new Staff
                {
                    OrganizationId = dto.OrganizationId,
                    AccountId = accountId,
                    Role = dto.Role,
                    JoinedDate = DateTime.UtcNow,
                    IsActive = true
                };

                await _staffRepo.AddAsync(staff);

                return (true, "Staff created successfully", staff.Id);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating staff: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateStaff(int staffId, UpdateStaffDto dto)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);
            if (staff == null)
                return (false, "Staff not found");

            if (dto.Role.HasValue)
                staff.Role = dto.Role.Value;

            if (dto.IsActive.HasValue)
                staff.IsActive = dto.IsActive.Value;

            if (staff.StaffAccount != null)
            {
                if (!string.IsNullOrEmpty(dto.Name))
                    staff.StaffAccount.Name = dto.Name;

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    var emailExists = await _accountRepo.ExistsAsync(a =>
                        a.Email == dto.Email && a.Id != staff.StaffAccount.Id);

                    if (emailExists)
                        return (false, "Email already in use by another account");

                    staff.StaffAccount.Email = dto.Email;
                }

                if (!string.IsNullOrEmpty(dto.NewPassword) || !string.IsNullOrEmpty(dto.ConfirmPassword))
                {
                    if (string.IsNullOrEmpty(dto.NewPassword) || string.IsNullOrEmpty(dto.ConfirmPassword))
                        return (false, "Both NewPassword and ConfirmPassword are required");

                    if (dto.NewPassword != dto.ConfirmPassword)
                        return (false, "New password and confirm password do not match");

                    staff.StaffAccount.PasswordHash = _passwordHelper.HashPassword(dto.NewPassword);
                }
            }

            await _staffRepo.UpdateAsync(staff);
            return (true, "Staff updated successfully");
        }

        public async Task<(bool Success, string Message)> DeleteStaff(int staffId)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);
            if (staff == null)
                return (false, "Staff not found");

            await _staffRepo.DeleteAsync(staff);
            return (true, "Staff deleted successfully");
        }
    }
}