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
        private readonly OrganizationRepo _organRepo;
        private readonly AccountRepo _accountRepo;
        private readonly PasswordHelper _passwordHelper;

        public StaffService(StaffRepo staffRepo, AccountService accountService, OrganizationRepo organRepo,
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
            return staffs.Select(MapToViewStaffDto).ToList();
        }

        public async Task<ViewStaffDto?> GetStaffById(int id)
        {
            var staff = await _staffRepo.GetByIdAsync(id);
            return MapToViewStaffDto(staff);
        }

        public async Task<List<ViewStaffDto>> GetStaffByOrganId(int organId)
        {
            var staffs = await _staffRepo.GetStaffByOrganId(organId);
            return staffs.Select(MapToViewStaffDto).ToList();
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
            {
                staff.IsActive = dto.IsActive.Value;
                if (!dto.IsActive.Value)
                    staff.LeftAt = DateTime.UtcNow; 
            }

            if (staff.Account != null)
            {
                if (!string.IsNullOrEmpty(dto.Name))
                    staff.Account.Name = dto.Name;

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    if (staff.Account.Email != dto.Email)
                    {
                        var emailExists = await _accountRepo.ExistsAsync(a =>
                            a.Email == dto.Email && a.Id != staff.Account.Id);

                        if (emailExists)
                            return (false, "Email already in use by another account");

                        staff.Account.Email = dto.Email;
                    }
                }

                if (!string.IsNullOrEmpty(dto.PhoneNumber))
                    staff.Account.PhoneNumber = dto.PhoneNumber;

                if (dto.DateOfBirth.HasValue)
                    staff.Account.DateOfBirth = dto.DateOfBirth.Value;

                if (dto.IsFemale.HasValue)
                    staff.Account.IsFemale = dto.IsFemale.Value;

                staff.Account.UpdatedAt = DateTime.UtcNow;
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

        private ViewStaffDto MapToViewStaffDto(Staff staff)
        {
            return new ViewStaffDto
            {
                Id = staff.Id,
                OrganizationId = staff.OrganizationId,
                OrganizationName = staff.Organization?.Name, 
                AccountId = staff.AccountId,
                Name = staff.Account?.Name ?? string.Empty, 
                Email = staff.Account?.Email ?? string.Empty,
                StaffRole = staff.Role.ToString(), 
                JoinedDate = staff.JoinedAt 
            };
        }
    }
}