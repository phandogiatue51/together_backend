using Together.DTOs.Certi;
using Together.DTOs.Organ;
using Together.DTOs.Pro;
using Together.DTOs.Staf;
using Together.DTOs.User;
using Together.Helpers;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class OrganizationService
    {
        private readonly OrganizationRepo _organRepo;
        private readonly CloudinaryService _imageStorageService;
        private readonly StaffService _staffService;

        public OrganizationService(OrganizationRepo organRepo, CloudinaryService cloudinaryImageService, StaffService staffService)
        {
            _organRepo = organRepo;
            _imageStorageService = cloudinaryImageService;
            _staffService = staffService;
        }

        public async Task<List<ViewOrganDto>> GetAllOrgans()
        {
            var organs = await _organRepo.GettAll();

            return organs.Select(MapToViewOrganDto).ToList();
        }

        public async Task<ViewOrganDto?> GetOrganById(int id)
        {
            var organs = await _organRepo.GetByIdAsync(id);

            if (organs == null)
                return null;

            return MapToViewOrganDto(organs);
        }

        public async Task<(bool Success, string Message)> CreateOrgan(CreateOrganDto dto, IFormFile? imageFile = null)
        {
            var logoFile = imageFile ?? dto.ImageFile;

            var organ = new Organization
            {
                Name = dto.Name,
                Description = dto.Description,
                Website = dto.Website,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Type = dto.Type,
            };

            if (logoFile != null && logoFile.Length > 0)
            {
                organ.LogoUrl = await _imageStorageService.UploadImageAsync(logoFile);
            }

            await _organRepo.AddAsync(organ);
            return (true, "Organization created successfully!");
        }

        public async Task<(bool Success, string Message, int? OrganizationId, int? StaffId)>
            CreateOrganWithManager(CreateOrganWithManagerDto dto, IFormFile? imageFile = null)
        {
            try
            {
                var organization = new Organization
                {
                    Name = dto.Name!,
                    Description = dto.Description!,
                    Type = dto.Type,
                    Website = dto.Website!,
                    Email = dto.Email!,
                    PhoneNumber = dto.PhoneNumber!,
                    Address = dto.Address!,
                };

                if (imageFile != null)
                {
                    organization.LogoUrl = await _imageStorageService.UploadImageAsync(imageFile);
                }

                await _organRepo.AddAsync(organization);

                var createUserDto = new CreateUserDto
                {
                    Name = dto.Manager.Name,
                    Email = dto.Manager.Email,
                    Password = dto.Manager.Password,
                    PhoneNumber = dto.Manager.PhoneNumber,
                    DateOfBirth = dto.Manager.DateOfBirth,
                    IsFemale = dto.Manager.IsFemale
                };

                var createStaffDto = new CreateStaffDto
                {
                    NewAccount = createUserDto,
                    OrganizationId = organization.Id,
                    Role = dto.Manager.Role
                };

                var staffResult = await _staffService.CreateStaff(createStaffDto);

                if (!staffResult.Success)
                {
                    await _organRepo.DeleteAsync(organization);

                    return (false,
                        $"Organization created but failed to create manager: {staffResult.Message}",
                        null, null);
                }

                return (true,
                    "Organization registration submitted successfully. Pending admin approval.",
                    organization.Id,
                    staffResult.StaffId);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating organization: {ex.Message}", null, null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateOrgan(int id, CreateOrganDto dto, IFormFile newImageFile = null)
        {
            var existing = await _organRepo.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Organization not found!");
            }
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Website = dto.Website;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.Address = dto.Address;

            if (newImageFile != null && newImageFile.Length > 0)
            {
                existing.LogoUrl = await _imageStorageService.UpdateImageAsync(
                    existing.LogoUrl,
                    newImageFile);
            }
            await _organRepo.UpdateAsync(existing);
            return (true, "Organization updated successfully!");
        }

        public async Task<(bool Success, string Message)> DeleteOrgan(int id)
        {
            var organ = await _organRepo.GetByIdAsync(id);
            if (organ == null)
            {
                return (false, "Organization not found!");
            }

            if (!string.IsNullOrEmpty(organ.LogoUrl))
            {
                await _imageStorageService.DeleteImageAsync(organ.LogoUrl);
            }

            await _organRepo.DeleteAsync(organ);
            return (true, "Organization deleted successfully!");
        }

        public async Task<(bool Success, string Message)> VerifyOrgan(int id, VerifyOrganDto dto)
        {
            var organ = await _organRepo.GetByIdAsync(id);
            if (organ == null)
            {
                return (false, "Organization not found!");
            }

            organ.Status = dto.Status;
            if (dto.Status == OrganzationStatus.Rejected)
            {
                if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                {
                    return (false, "Rejection reason must be provided when rejecting an organization.");
                }
            }

            await _organRepo.UpdateAsync(organ);
            return (true, "Organization status updated successfully!");
        }

        public async Task<List<ViewOrganDto>> GetOrgansByFilter(OrganFilterDto dto)
        {
            var organs = await _organRepo.GetByFilterAsync(dto);
            return organs.Select(MapToViewOrganDto).ToList();
        }

        private ViewOrganDto MapToViewOrganDto(Organization organization)
        {
            return new ViewOrganDto
            {
                Id = organization.Id,
                Name = organization.Name,
                Description = organization.Description,
                Website = organization.Website,
                Email = organization.Email,
                PhoneNumber = organization.PhoneNumber,
                Address = organization.Address,
                LogoUrl = organization.LogoUrl,
                Type = organization.Type,
                TypeName = organization.Type.ToFriendlyName(),
                Status = organization.Status,
                StatusName = organization.Status.ToFriendlyName(),
                RejectionReason = organization.RejectionReason,
                CreateAt = organization.CreatedAt
            };
        }
    }
}   
