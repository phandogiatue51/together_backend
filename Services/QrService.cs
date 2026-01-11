using Microsoft.Extensions.Caching.Memory;
using QRCoder;
using Together.DTOs.App;
using Together.DTOs.Record;
using Together.Models;
using Together.Repositories;
using static Together.DTOs.Record.QrDto;

namespace Together.Services
{
    public class QrService
    {
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _env;
        private readonly ProjectRepo _projectRepo;
        private readonly ApplicationRepo _applicationRepo;
        private readonly HourRepo _hourRepo;
        private readonly AccountRepo _accountRepo;
        private readonly UnitOfWork _unitOfWork;

        public QrService(IMemoryCache cache, IWebHostEnvironment env, ProjectRepo projectRepo,
            ApplicationRepo applicationRepo, HourRepo hourRepo, AccountRepo accountRepo, UnitOfWork unitOfWork)
        {
            _cache = cache;
            _env = env;
            _projectRepo = projectRepo;
            _applicationRepo = applicationRepo;
            _hourRepo = hourRepo;
            _accountRepo = accountRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<QrResponseDto> GenerateCheckInQrCodeAsync(GenerateQrDto dto)
        {
            return await GenerateQrCodeAsync(dto, "checkin");
        }

        public async Task<QrResponseDto> GenerateCheckOutQrCodeAsync(GenerateQrDto dto)
        {
            return await GenerateQrCodeAsync(dto, "checkout");
        }

        private async Task<QrResponseDto> GenerateQrCodeAsync(GenerateQrDto dto, string actionType)
        {
            var project = await _projectRepo.GetByIdAsync(dto.ProjectId);
            if (project == null)
                throw new Exception("Project not found");

            var token = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddHours(2);

            _cache.Set($"qr-{token}", new QrCacheData
            {
                ProjectId = dto.ProjectId,
                ExpiresAt = expiresAt,
                ActionType = actionType,
                CreatedAt = DateTime.UtcNow
            }, expiresAt - DateTime.UtcNow);

            var qrContent = $"https://exe-201-togethers-projects-06b082f5.vercel.app/attendance/{token}?action={actionType}";

            string qrImageBase64;
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q))
            using (var qrCode = new PngByteQRCode(qrData))
            {
                var qrBytes = qrCode.GetGraphic(20);
                qrImageBase64 = Convert.ToBase64String(qrBytes);
            }

            return new QrResponseDto
            {
                QrToken = token,
                QrImageBase64 = $"data:image/png;base64,{qrImageBase64}",
                ExpiresAt = expiresAt,
                ProjectName = project.Title,
                ActionType = actionType
            };
        }

        public async Task<ScanResultDto> ProcessQrScanAsync(QrActionDto dto, int volunteerId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var cacheKey = $"qr-{dto.QrToken}";
                if (!_cache.TryGetValue(cacheKey, out QrCacheData tokenData))
                    throw new Exception("Invalid or expired QR code");

                if (tokenData.ActionType == "checkout")
                {
                    _cache.Remove(cacheKey);
                }

                int projectId = tokenData.ProjectId;
                var actionType = tokenData.ActionType;

                var filter = new AppFilterDto
                {
                    ProjectId = projectId,
                    Status = null,
                    VolunteerId = volunteerId,
                    OrganizationId = null
                };

                var applications = await _applicationRepo.GetByFilterAsync(filter);
                var application = applications.FirstOrDefault();

                if (application == null)
                    throw new Exception("You are not approved for this project!");

                var actionTime = dto.ActionTime ?? DateTime.UtcNow;
                var today = actionTime.Date;

                var activeRecord = await _hourRepo.GetActiveRecordAsync(application.Id, today);

                if (actionType == "checkin")
                {
                    if (activeRecord != null)
                        throw new Exception("You are already checked in for today!");

                    if (activeRecord.CheckOut.HasValue)
                        throw new Exception("You have already checked out for today!");

                    var record = new VolunteerHour
                    {
                        VolunteerApplicationId = application.Id,
                        CheckIn = actionTime
                    };

                    await _hourRepo.AddAsync(record);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    return new ScanResultDto
                    {
                        Action = "checkin",
                        Message = $"Checked in to {application.Project.Title}",
                        Time = actionTime,
                        RecordId = record.RecordId,
                        ProjectId = projectId,
                        ProjectName = application.Project.Title 
                    };
                }
                else
                {
                    if (activeRecord == null)
                        throw new Exception("No active check-in found. Please check in first.");

                    if (activeRecord.CheckOut.HasValue)
                        throw new Exception("Already checked out");

                    if (actionTime <= activeRecord.CheckIn)
                        throw new Exception("Check-out time must be after check-in");

                    if (actionTime.Date != activeRecord.CheckIn)
                        throw new Exception("Check-out must be on the same day as check-in");

                    activeRecord.CheckOut = actionTime;

                    var timeSpan = activeRecord.CheckOut.Value - activeRecord.CheckIn.Value;
                    activeRecord.Hours = (decimal)timeSpan.TotalHours;

                    await _hourRepo.UpdateAsync(activeRecord);

                    var account = await _accountRepo.GetByIdAsync(volunteerId);
                    account.Hour += activeRecord.Hours;
                    await _accountRepo.UpdateAsync(account);

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitAsync();

                    return new ScanResultDto
                    {
                        Action = "checkout",
                        Message = $"Checked out from {application.Project.Title}",
                        HoursWorked = activeRecord.Hours,
                        TotalHours = account.Hour,
                        CheckInTime = activeRecord.CheckIn,
                        CheckOutTime = activeRecord.CheckOut,
                        ProjectId = projectId, 
                        ProjectName = application.Project.Title 
                    };
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        // Cleanup expired QR codes
        public void CleanupExpiredQrCodes()
        {
            // This could be called periodically (e.g., via background service)
            // For now, it's available for manual cleanup
            // In production, consider using IMemoryCache with callbacks or a background service
        }

        public List<QrCacheData> GetActiveQrCodes()
        {
            var result = new List<QrCacheData>();

            // Note: This is a simplified approach. In production, you might need 
            // a different cache implementation to list all keys
            // or maintain a separate list of active tokens

            return result;
        }
    }
}