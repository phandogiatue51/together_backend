using Together.DTOs.App;
using Together.DTOs.Certi;
using Together.Helpers;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class ApplicationService
    {
        private readonly ApplicationRepo _applicationRepo;
        private readonly StaffRepo _staffRepo;
        private readonly CertificateRepo _certificateRepo;
        private readonly AccountRepo _accountRepo;
        private readonly ProjectRepo _projectRepo;

        public ApplicationService(ApplicationRepo applicationRepo, StaffRepo staffRepo,
            CertificateRepo certificateRepo, AccountRepo accountRepo, ProjectRepo projectRepo)
        {
            _applicationRepo = applicationRepo;
            _staffRepo = staffRepo;
            _certificateRepo = certificateRepo;
            _accountRepo = accountRepo;
            _projectRepo = projectRepo;
        }

        public async Task<List<ViewAppDto>> GetAllApplicationsAsync()
        {
            var apps = await _applicationRepo.GetAllAsync();
            return apps.Select(MapToViewAppDto).ToList();
        }

        public async Task<ViewAppDto?> GetApplicationByIdAsync(int id)
        {
            var app = await _applicationRepo.GetByIdAsync(id);
            if (app == null)
                return null;
            return MapToViewAppDto(app);
        }

        public async Task<(bool Success, string Message)> CreateApplicationAsync(CreateAppDto dto)
        {
            var existing = await _applicationRepo.GetExistingApplicationAsync(dto.ProjectId, dto.VolunteerId);

            if (existing != null && (existing.Status == ApplicationStatus.Pending || existing.Status == ApplicationStatus.Approved))
            {
                return (false, "You have already applied for this project.");
            }

            var project = await _projectRepo.GetByIdAsync(dto.ProjectId);
            if (project == null)
            {
                return (false, "Project not found.");
            }

            var volunteer = await _accountRepo.GetByIdAsync(dto.VolunteerId);
            if (volunteer == null)
            {
                return (false, "Volunteer account not found.");
            }

            List<Certificate> selectedCertificates = new();

            if (dto.SelectedCertificateIds != null && dto.SelectedCertificateIds.Any())
            {
                selectedCertificates = await _certificateRepo.GetCertificatesByIdsAsync(dto.SelectedCertificateIds);
            }

            var newApp = new VolunteerApplication
            {
                ProjectId = dto.ProjectId,
                VolunteerId = dto.VolunteerId,
                RelevantExperience = dto.RelevantExperience,
                AppliedAt = DateTime.UtcNow,
                SelectedCertificates = selectedCertificates
            };

            await _applicationRepo.AddAsync(newApp);
            return (true, "Application created successfully.");
        }

        public async Task<(bool Success, string Message)> UpdateApplicationAsync(int id, UpdateAppDto dto)
        {
            var app = await _applicationRepo.GetByIdAsync(id);
            if (app == null)
                return (false, "Application not found!");

            if (app.Status == ApplicationStatus.Approved || app.Status == ApplicationStatus.Rejected)
            {
                return (false, "Cannot update an application that has already been reviewed.");
            }

            if (dto.RelevantExperience != null)
            {
                app.RelevantExperience = dto.RelevantExperience;
            }

            if (dto.SelectedCertificateIds != null)
            {
                List<Certificate> selectedCertificates = new();

                if (dto.SelectedCertificateIds.Any())
                {
                    selectedCertificates = await _certificateRepo
                        .GetCertificatesByIdsAsync(dto.SelectedCertificateIds);
                }

                app.SelectedCertificates = selectedCertificates;
            }

            await _applicationRepo.UpdateAsync(app);
            return (true, "Application updated successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteApplicationAsync(int id)
        {
            var app = await _applicationRepo.GetByIdAsync(id);
            if (app == null)
                return (false, "Application not found!");
            await _applicationRepo.DeleteAsync(app);
            return (true, "Application deleted successfully.");
        }

        public async Task<List<ViewAppDto>> GetApplicationsByFilterAsync(AppFilterDto filter)
        {
            var apps = await _applicationRepo.GetByFilterAsync(filter);
            return apps.Select(MapToViewAppDto).ToList();
        }

        public async Task<(bool Success, string Message)> ReviewApplicationAsync(int id, ReviewAppDto dto)
        {
            var app = await _applicationRepo.GetByIdAsync(id);
            if (app == null)
                return (false, "Application not found!");

            var staff = await _staffRepo.GetByIdAsync(dto.ReviewedByStaffId ?? 0);
            if (staff == null || staff.Role != StaffRole.Reviewer)
            {
                return (false, "Only staff with Reviewer role can review applications.");
            }

            if (app.Status != ApplicationStatus.Pending)
            {
                return (false, "Only pending applications can be reviewed.");
            }

            app.Status = dto.Status;
            app.ReviewedAt = DateTime.UtcNow;
            app.ReviewedByStaffId = dto.ReviewedByStaffId;

            if (app.Status == ApplicationStatus.Rejected)
            {
                app.RejectionReason = dto.RejectionReason;
            }

            await _applicationRepo.UpdateAsync(app);
            return (true, "Application reviewed successfully.");
        }

        private ViewAppDto MapToViewAppDto(VolunteerApplication a)
        {
            return new ViewAppDto
            {
                Id = a.Id,
                ProjectId = a.ProjectId,
                ProjectTitle = a.Project?.Title,
                OrganizationId = a.Project?.OrganizationId,
                OrganizationName = a.Project?.Organization?.Name,
                VolunteerId = a.VolunteerId,
                VolunteerName = a.Volunteer?.Name,
                Status = a.Status,
                StatusName = a.Status.ToFriendlyName(),
                RelevantExperience = a.RelevantExperience,
                AppliedAt = a.AppliedAt,
                ReviewedAt = a.ReviewedAt,
                ReviewedByStaffId = a.ReviewedByStaffId,
                RejectionReason = a.RejectionReason,
                Feedback = a.Feedback,
                SelectedCertificates = a.SelectedCertificates.Select(certificate => new ViewCertiDto
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
                    //Status = certificate.Status,
                    //StatusName = certificate.Status.ToString(),
                    //CreatedAt = certificate.CreatedAt,
                    //VerifiedAt = certificate.VerifiedAt,
                    //VerifiedByAdminId = certificate.VerifiedByAdminId
                }).ToList() 
            };
        }
    }
}