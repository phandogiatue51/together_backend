
using Together.DTOs.App;
using Together.Models;
using Together.Repositories;
using Microsoft.Extensions.Caching.Memory;

using static global::Together.DTOs.Record.AttendanceDto;

namespace Together.Services
{
    public class AttendanceService
    {
        private readonly IMemoryCache _cache;
        private readonly ProjectRepo _projectRepo;
        private readonly ApplicationRepo _applicationRepo;
        private readonly HourRepo _hourRepo;
        private readonly AccountRepo _accountRepo;
        private readonly UnitOfWork _unitOfWork;

        public AttendanceService(
            IMemoryCache cache,
            ProjectRepo projectRepo,
            ApplicationRepo applicationRepo,
            HourRepo hourRepo,
            AccountRepo accountRepo,
            UnitOfWork unitOfWork)
        {
            _cache = cache;
            _projectRepo = projectRepo;
            _applicationRepo = applicationRepo;
            _hourRepo = hourRepo;
            _accountRepo = accountRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<GenerateCodeResponseDto> GenerateAttendanceCodeAsync(int projectId, string actionType)
        {
            var project = await _projectRepo.GetByIdAsync(projectId);
            if (project == null)
                throw new Exception("Project not found");

            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            while (_cache.TryGetValue($"attendance-{code}", out _))
            {
                code = random.Next(100000, 999999).ToString();
            }

            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            var codeData = new AttendanceCodeData
            {
                Code = code,
                ProjectId = projectId,
                ActionType = actionType,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };

            _cache.Set($"attendance-{code}", codeData, expiresAt - DateTime.UtcNow);

            return new GenerateCodeResponseDto
            {
                Code = code,
                ExpiresAt = expiresAt,
                ProjectName = project.Title,
                ActionType = actionType
            };
        }

        public async Task<VerifyCodeResponseDto> VerifyAttendanceCodeAsync(string code, int volunteerId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var cacheKey = $"attendance-{code}";
                if (!_cache.TryGetValue(cacheKey, out AttendanceCodeData codeData))
                    throw new Exception("Mã không hợp lệ hoặc đã hết hạn");

                _cache.Remove(cacheKey);

                var projectId = codeData.ProjectId;
                var actionType = codeData.ActionType;
                var actionTime = DateTime.UtcNow;

                var filter = new AppFilterDto
                {
                    ProjectId = projectId,
                    Status = ApplicationStatus.Approved,
                    VolunteerId = volunteerId,
                    OrganizationId = null
                };

                var applications = await _applicationRepo.GetByFilterAsync(filter);
                var application = applications.FirstOrDefault();

                if (application == null)
                    throw new Exception("Bạn chưa được phép tham gia dự án này!");

                var today = actionTime.Date;

                var activeRecord = await _hourRepo.GetActiveRecordAsync(application.Id, today);

                if (actionType == "checkin")
                {
                    if (activeRecord != null)
                        throw new Exception("Bạn đã điểm danh vào hôm nay!");

                    var record = new VolunteerHour
                    {
                        VolunteerApplicationId = application.Id,
                        CheckIn = actionTime
                    };

                    await _hourRepo.AddAsync(record);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    return new VerifyCodeResponseDto
                    {
                        Action = "checkin",
                        Message = $"Đã điểm danh vào {application.Project.Title}",
                        Time = actionTime,
                        ProjectId = projectId,
                        ProjectName = application.Project.Title
                    };
                }
                else 
                {
                    if (activeRecord == null)
                        throw new Exception("Chưa điểm danh vào. Vui lòng điểm danh vào trước.");

                    if (activeRecord.CheckOut.HasValue)
                        throw new Exception("Đã điểm danh ra rồi!");

                    if (actionTime <= activeRecord.CheckIn)
                        throw new Exception("Thời gian điểm danh ra phải sau thời gian điểm danh vào");

                    activeRecord.CheckOut = actionTime;
                    var timeSpan = activeRecord.CheckOut.Value - activeRecord.CheckIn.Value;
                    activeRecord.Hours = Math.Round((decimal)timeSpan.TotalHours, 2);

                    await _hourRepo.UpdateAsync(activeRecord);

                    var account = await _accountRepo.GetByIdAsync(volunteerId);
                    account.Hour += activeRecord.Hours;
                    await _accountRepo.UpdateAsync(account);

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    return new VerifyCodeResponseDto
                    {
                        Action = "checkout",
                        Message = $"Đã điểm danh ra từ {application.Project.Title}",
                        Time = actionTime,
                        ProjectId = projectId,
                        ProjectName = application.Project.Title,
                        HoursWorked = activeRecord.Hours,
                        TotalHours = account.Hour
                    };
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public void CleanupExpiredCodes()
        {
        }
    }
}