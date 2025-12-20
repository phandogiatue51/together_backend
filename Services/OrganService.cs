using Together.DTOs.Organ;
using Together.Helpers;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class OrganService
    {
        private readonly OrganRepo _organRepo;
        private readonly CloudinaryService _imageStorageService;
        public OrganService(OrganRepo organRepo, CloudinaryService cloudinaryImageService)
        {
            _organRepo = organRepo;
            _imageStorageService = cloudinaryImageService;
        }

        public async Task<List<ViewOrganDto>> GetAllOrgans()
        {
            var organs = await _organRepo.GettAll();

            return organs.Select(m => new ViewOrganDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                LogoUrl = m.LogoUrl,
                Website = m.Website,
                ContactEmail = m.ContactEmail,
                ContactPhone = m.ContactPhone,
                Address = m.Address
            }).ToList();
        }

        public async Task<ViewOrganDto?> GetOrganById(int id)
        {
            var organs = await _organRepo.GetByIdAsync(id);

            if (organs == null)
                return null;

            return new ViewOrganDto
            {
                Id = organs.Id,
                Name = organs.Name,
                Description = organs.Description,
                LogoUrl = organs.LogoUrl,
                Website = organs.Website,
                ContactEmail = organs.ContactEmail,
                ContactPhone = organs.ContactPhone,
                Address = organs.Address
            };
        }

        public async Task<(bool Success, string Message)> CreateOrgan(CreateOrganDto dto, IFormFile? imageFile = null)
        {
            var logoFile = imageFile ?? dto.ImageFile;

            var organ = new Organization
            {
                Name = dto.Name,
                Description = dto.Description,
                Website = dto.Website,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                Address = dto.Address
            };

            if (logoFile != null && logoFile.Length > 0)
            {
                organ.LogoUrl = await _imageStorageService.UploadImageAsync(logoFile);
            }

            await _organRepo.AddAsync(organ);
            return (true, "Organization created successfully!");
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
            existing.ContactEmail = dto.ContactEmail;
            existing.ContactPhone = dto.ContactPhone;
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
            await _organRepo.DeleteAsync(organ);
            return (true, "Organization deleted successfully!");
        }
    }
}
