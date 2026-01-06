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

        public async Task<(bool Success, string Message, int? id)> CreateApplicationAsync(CreateAppDto dto)
        {
            var existing = await _applicationRepo.GetExistingApplicationAsync(dto.ProjectId, dto.VolunteerId);

            if (existing != null && (existing.Status == ApplicationStatus.Pending || existing.Status == ApplicationStatus.Approved))
            {
                return (false, "Bạn đã đăng ký chương trình này!", null);
            }

            var project = await _projectRepo.GetByIdAsync(dto.ProjectId);
            if (project == null)
            {
                return (false, "Không tìm thấy chương trình!", null);
            }

            var volunteer = await _accountRepo.GetByIdAsync(dto.VolunteerId);
            if (volunteer == null)
            {
                return (false, "Không tìm thấy tài khoản!", null);
            }

            var newApp = new VolunteerApplication
            {
                ProjectId = dto.ProjectId,
                VolunteerId = dto.VolunteerId,
                RelevantExperience = dto.RelevantExperience,
                AppliedAt = DateTime.UtcNow,
                SelectedCertificateId = dto.SelectedCertificateId,

            };

            await _applicationRepo.AddAsync(newApp);
            return (true, "Đăng ký chương trình thành công! Vui lòng đợi tổ chức phê duyệt!", newApp.Id);
        }

        public async Task<(bool Success, string Message)> UpdateApplicationAsync(int id, UpdateAppDto dto)
        {
            var app = await _applicationRepo.GetByIdAsync(id);
            if (app == null)
                return (false, "Không tìm thấy đơn đăng ký!");

            if (app.Status == ApplicationStatus.Approved || app.Status == ApplicationStatus.Rejected)
            {
                return (false, "Không thể cập nhật đơn đăng ký");
            }

            if (dto.RelevantExperience != null)
            {
                app.RelevantExperience = dto.RelevantExperience;
            }

            dto.SelectedCertificateId = app.SelectedCertificateId;

            await _applicationRepo.UpdateAsync(app);
            return (true, "Cập nhật đơn đăng ký thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteApplicationAsync(int id)
        {
            var app = await _applicationRepo.GetByIdAsync(id);
            if (app == null)
                return (false, "Không tìm thấy đơn đăng ký!");
            await _applicationRepo.DeleteAsync(app);
            return (true, "Xóa đơn đăng ký thành công");
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
                return (false, "Không tìm thấy đơn đăng ký!");

            var staff = await _staffRepo.GetByIdAsync(dto.ReviewedByStaffId ?? 0);
            if (staff == null || staff.Role == StaffRole.Employee)
            {
                return (false, "Bạn không có quyền phê duyệt đơn đăng ký");
            }

            if (app.Status != ApplicationStatus.Pending)
            {
                return (false, "Chỉ có thể phê duyệt đơn đăng ký trong trạng thái chờ!");
            }

            app.Status = dto.Status;
            app.ReviewedAt = DateTime.UtcNow;
            app.ReviewedByStaffId = dto.ReviewedByStaffId;

            if (app.Status == ApplicationStatus.Rejected)
            {
                app.RejectionReason = dto.RejectionReason;
            }
            else
            {
                var project = await _projectRepo.GetByIdAsync(app.ProjectId);
                project.CurrentVolunteers += 1;
            }

            await _projectRepo.UpdateAsync(app.Project!);
            await _applicationRepo.UpdateAsync(app);
            return (true, "Phê duyệt đơn đăng ký thành công!");
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
                SelectedCertificate = a.SelectedCertificate != null ? new ViewCertiDto
                {
                    Id = a.SelectedCertificate.Id,
                    CertificateName = a.SelectedCertificate.CertificateName,
                    CertificateNumber = a.SelectedCertificate.CertificateNumber,
                    ImageUrl = a.SelectedCertificate.ImageUrl,
                    Description = a.SelectedCertificate.Description,
                    IssuingOrganization = a.SelectedCertificate.IssuingOrganization,
                    CategoryId = a.SelectedCertificate.CategoryId,
                    IssueDate = a.SelectedCertificate.IssueDate,
                    ExpiryDate = a.SelectedCertificate.ExpiryDate
                } : null
            };
        }
    }
}