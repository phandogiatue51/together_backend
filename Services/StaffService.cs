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
        private readonly CloudinaryService _cloudinaryService;

        public StaffService(StaffRepo staffRepo, AccountService accountService, OrganizationRepo organRepo,
            AccountRepo accountRepo, PasswordHelper passwordHelper, CloudinaryService cloudinaryService)
        {
            _staffRepo = staffRepo;
            _accountService = accountService;
            _organRepo = organRepo;
            _accountRepo = accountRepo;
            _passwordHelper = passwordHelper;
            _cloudinaryService = cloudinaryService;
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

        public async Task<ViewStaffDto?> GetStaffByAccountId(int accountId)
        {
            var staff = await _staffRepo.GetByAccountId(accountId);
            return MapToViewStaffDto(staff);
        }

        public async Task<(bool Success, string Message, int? StaffId)> CreateStaff(CreateStaffDto dto)
        {
            try
            {
                var organization = await _organRepo.GetByIdAsync(dto.OrganizationId);
                if (organization == null)
                    return (false, "Không tìm thấy tổ chức!", null);

                int accountId;

                var accountResult = await _accountService.CreateAccount(dto.NewAccount, AccountRole.Staff, dto.NewAccount.ProfileImageUrl);

                if (!accountResult.Success)
                    return (false, $"Không thể tạo tài khoản tổ chức: {accountResult.Message}!", null);

                accountId = accountResult.AccountId;

                var staff = new Staff
                {
                    OrganizationId = dto.OrganizationId,
                    AccountId = accountId,
                    Role = dto.Role,
                };

                await _staffRepo.AddAsync(staff);

                return (true, "Tạo tài khoản tổ chức thành công!", staff.Id);
            }
            catch (Exception ex)
            {
                return (false, $"Không thể tạo tài khoản tổ chức: {ex.Message}!", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateStaff(int staffId, UpdateStaffDto dto)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);
            if (staff == null)
                return (false, "Không tìm thấy nhân viên!");

            if (dto.Role.HasValue)
                staff.Role = dto.Role.Value;

            if (dto.IsActive.HasValue)
            {
                staff.IsActive = dto.IsActive.Value;
                if (!dto.IsActive.Value)
                    staff.LeftAt = DateTime.UtcNow;
                else if (dto.IsActive.Value && staff.LeftAt.HasValue)
                    staff.LeftAt = null; 
            }

            if (staff.Account != null)
            {
                if (dto.ProfileImageUrl != null)
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(dto.ProfileImageUrl);
                    staff.Account.ProfileImageUrl = imageUrl;
                }

                if (!string.IsNullOrEmpty(dto.Name))
                    staff.Account.Name = dto.Name;

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    if (staff.Account.Email != dto.Email)
                    {
                        var emailExists = await _accountRepo.ExistsAsync(a =>
                            a.Email == dto.Email && a.Id != staff.Account.Id);

                        if (emailExists)
                            return (false, "Email đã được sử dụng ở tài khoản khác!");

                        staff.Account.Email = dto.Email;
                    }
                }

                if (!string.IsNullOrEmpty(dto.PhoneNumber))
                    staff.Account.PhoneNumber = dto.PhoneNumber;

                if (dto.DateOfBirth.HasValue)
                    staff.Account.DateOfBirth = dto.DateOfBirth.Value;

                if (dto.IsFemale.HasValue)
                    staff.Account.IsFemale = dto.IsFemale.Value;

                if (!string.IsNullOrEmpty(dto.Bio))
                    staff.Account.Bio = dto.Bio;

                staff.Account.UpdatedAt = DateTime.UtcNow;
            }

            await _staffRepo.UpdateAsync(staff);
            return (true, "Cập nhật tài khoản tổ chức thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteStaff(int staffId)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);
            if (staff == null)
                return (false, "Không tìm thấy nhân viên");

            await _staffRepo.DeleteAsync(staff);
            return (true, "Xóa nhân viên thành công!");
        }

        public async Task<List<ViewStaffDto>> GetStaffByFilterAsync(StaffFilterDto dto)
        {
            var staffs = await _staffRepo.GetByFilterAsync(dto);
            return staffs.Select(MapToViewStaffDto).ToList();
        }

        public async Task<(bool Success, string Message)> ChangeStatus(int staffId)
        {
            var staff = await _staffRepo.GetByIdAsync(staffId);
            if (staff == null)
                return (false, "Không tìm thấy nhân viên!");

            staff.IsActive = !staff.IsActive;

            if (!staff.IsActive)
            {
                staff.LeftAt = DateTime.UtcNow;
            }
            else if (staff.IsActive && staff.LeftAt.HasValue)
            {
                staff.LeftAt = null;
            }

            await _staffRepo.UpdateAsync(staff);
            return (true, "Cập nhật trạng thái thành công!");
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
                PhoneNumber = staff.Account?.PhoneNumber ?? string.Empty,
                DateOfBirth = staff.Account?.DateOfBirth ?? default(DateOnly),
                ProfileImageUrl = staff.Account?.ProfileImageUrl,
                Role = staff.Role,
                StaffRole = staff.Role.ToFriendlyName(),
                IsActive = staff.IsActive,
                JoinedDate = staff.JoinedAt,
                LeftDate = staff.LeftAt
            };
        }
    }
}