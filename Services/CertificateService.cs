using Together.DTOs.Certi;
using Together.Helpers;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class CertificateService
    {
        private readonly CertificateRepo _certificateRepo;
        private readonly AccountRepo _accountRepo;
        private readonly CategoryRepo _categoryRepo;
        private readonly CloudinaryService _imageStorageService;

        public CertificateService(CertificateRepo certificateRepo, AccountRepo accountRepo, CategoryRepo categoryRepo, CloudinaryService imageStorageService)
        {
            _certificateRepo = certificateRepo;
            _accountRepo = accountRepo;
            _categoryRepo = categoryRepo;
            _imageStorageService = imageStorageService;
        }

        public async Task<List<ViewCertiDto>> GetAllCertificatesAsync()
        {
            var certificates = await _certificateRepo.GetAllAsync();
            return certificates.Select(MapToViewCertiDto).ToList();
        }

        public async Task<List<ViewCertiDto>> GetCertificatesByAccountIdAsync(int accountId)
        {
            var certificates = await _certificateRepo.GetByAccountIdAsync(accountId);
            return certificates.Select(MapToViewCertiDto).ToList();
        }

        public async Task<ViewCertiDto?> GetCertificateByIdAsync(int id)
        {
            var certificate = await _certificateRepo.GetByIdAsync(id);
            return certificate == null ? null : MapToViewCertiDto(certificate);
        }

        public async Task<(bool Success, string Message, int? CertificateId)> CreateCertificateAsync(
            CreateCertiDto dto, IFormFile imageFile)
        {
            try
            {
                var account = await _accountRepo.GetByIdAsync(dto.AccountId);
                if (account == null)
                    return (false, "Account not found.", null);

                var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
                if (category == null || !category.IsActive)
                    return (false, "Category not found or inactive.", null);

                if (imageFile == null || imageFile.Length == 0)
                    return (false, "Certificate image is required.", null);

                var imageUrl = await _imageStorageService.UploadImageAsync(imageFile);

                var certificate = new Certificate
                {
                    AccountId = dto.AccountId,
                    CertificateName = dto.CertificateName,
                    CategoryId = dto.CategoryId, 
                    IssuingOrganization = dto.IssuingOrganization,
                    CertificateNumber = dto.CertificateNumber,
                    IssueDate = dto.IssueDate,
                    ExpiryDate = dto.ExpiryDate,
                    Description = dto.Description,
                    ImageUrl = imageUrl,
                    Status = CertificateStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _certificateRepo.AddAsync(certificate);
                return (true, "Certificate uploaded successfully. Awaiting admin verification.", certificate.Id);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating certificate: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateCertificateAsync(
            int id, CreateCertiDto dto, IFormFile? imageFile)
        {
            try
            {
                var certificate = await _certificateRepo.GetByIdAsync(id);
                if (certificate == null)
                    return (false, "Certificate not found.");

                if (certificate.Status != CertificateStatus.Pending)
                    return (false, "Only pending certificates can be updated.");

                var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
                if (category == null || !category.IsActive)
                    return (false, "Category not found or inactive.");

                certificate.CertificateName = dto.CertificateName;
                certificate.CategoryId = dto.CategoryId;
                certificate.IssuingOrganization = dto.IssuingOrganization;
                certificate.CertificateNumber = dto.CertificateNumber;
                certificate.IssueDate = dto.IssueDate;
                certificate.ExpiryDate = dto.ExpiryDate;
                certificate.Description = dto.Description;

                if (imageFile != null && imageFile.Length > 0)
                {
                    certificate.ImageUrl = await _imageStorageService.UpdateImageAsync(
                        certificate.ImageUrl, imageFile);
                }

                certificate.Status = CertificateStatus.Pending;
                certificate.VerifiedAt = null;
                certificate.VerifiedByAdminId = null;

                await _certificateRepo.UpdateAsync(certificate);
                return (true, "Certificate updated successfully. Awaiting re-verification.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating certificate: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> VerifyCertificateAsync(int certificateId, int adminId, bool isVerified, string? rejectionReason = null)
        {
            try
            {
                var certificate = await _certificateRepo.GetByIdAsync(certificateId);
                if (certificate == null)
                    return (false, "Certificate not found.");

                var admin = await _accountRepo.GetByIdAsync(adminId);
                if (admin == null || admin.Role != AccountRole.Admin)
                    return (false, "You don't have permission!");

                if (isVerified)
                {
                    certificate.Status = CertificateStatus.Verified;
                    certificate.VerifiedAt = DateTime.UtcNow;
                    certificate.VerifiedByAdminId = adminId;
                }
                else
                {
                    certificate.Status = CertificateStatus.Rejected;
                    certificate.VerifiedAt = DateTime.UtcNow;
                    certificate.VerifiedByAdminId = adminId;
                }

                await _certificateRepo.UpdateAsync(certificate);
                return (true, $"Certificate {(isVerified ? "verified" : "rejected")} successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error verifying certificate: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteCertificateAsync(int id)
        {
            try
            {
                var certificate = await _certificateRepo.GetByIdAsync(id);
                if (certificate == null)
                    return (false, "Certificate not found.");

                if (!string.IsNullOrEmpty(certificate.ImageUrl))
                {
                    await _imageStorageService.DeleteImageAsync(certificate.ImageUrl);
                }

                await _certificateRepo.DeleteAsync(certificate);
                return (true, "Certificate deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error deleting certificate: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> VerifyCertificateAsync(int id, VerifyCertiDto dto)
        {
            try
            {
                var certificate = await _certificateRepo.GetByIdAsync(id);
                if (certificate == null)
                    return (false, "Certificate not found.");
                var admin = await _accountRepo.GetByIdAsync(dto.AdminId);
                if (admin == null || admin.Role != AccountRole.Admin)
                    return (false, "You don't have permission!");
                certificate.Status = dto.Status;
                certificate.VerifiedAt = DateTime.UtcNow;
                certificate.VerifiedByAdminId = dto.AdminId;
                await _certificateRepo.UpdateAsync(certificate);
                return (true, "Certificate verification status updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error verifying certificate: {ex.Message}");
            }
        }

        public async Task<List<ViewCertiDto>> FilterCertificatesAsync(CertiFilterDto filterDto)
        {
            var certificates = await _certificateRepo.GetFilteredAsync(filterDto);
            return certificates.Select(c => MapToViewCertiDto(c)).ToList();
        }


        private ViewCertiDto MapToViewCertiDto(Certificate certificate)
        {
            return new ViewCertiDto
            {
                Id = certificate.Id,
                AccountId = certificate.AccountId,
                CertificateName = certificate.CertificateName,
                CategoryId = certificate.CategoryId,
                CategoryName = certificate.Category?.Name, 
                IssuingOrganization = certificate.IssuingOrganization,
                CertificateNumber = certificate.CertificateNumber,
                IssueDate = certificate.IssueDate,
                ExpiryDate = certificate.ExpiryDate,
                Description = certificate.Description,
                ImageUrl = certificate.ImageUrl,
                Status = certificate.Status,
                CreatedAt = certificate.CreatedAt,
                VerifiedAt = certificate.VerifiedAt,
                VerifiedByAdminId = certificate.VerifiedByAdminId
            };
        }
    }
}