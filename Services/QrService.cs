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

        public async Task<QrResponseDto> GenerateQrCodeAsync(GenerateQrDto dto)
        {
            var project = await _projectRepo.GetByIdAsync(dto.ProjectId);
            if (project == null)
                throw new Exception("Project not found");


            var token = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddHours(dto.DurationHours ?? 24);

            _cache.Set($"qr-{token}", new
            {
                ProjectId = dto.ProjectId,
                ExpiresAt = expiresAt
            }, expiresAt - DateTime.UtcNow);

            var qrContent = $"together://checkin/{token}";
            // OR: $"https://yourapp.com/checkin/{token}" for web

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
                ProjectName = project.Title
            };
        }

        public async Task<ScanResultDto> ProcessQrScanAsync(QrActionDto dto, int volunteerId)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var cacheKey = $"qr-{dto.QrToken}";
                if (!_cache.TryGetValue(cacheKey, out object tokenData))
                    throw new Exception("Invalid or expired QR code");

                dynamic data = tokenData;
                int projectId = data.ProjectId;

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
                    throw new Exception("You are not approved for this project");

                var actionTime = dto.ActionTime ?? DateTime.UtcNow;
                var today = actionTime.Date;

                var activeRecord = await _hourRepo.GetActiveRecordAsync(application.Id, today);

                if (activeRecord == null)
                {
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
                        Action = "check-in",
                        Message = $"Checked in to {application.Project.Title}",
                        Time = actionTime,
                        RecordId = record.RecordId
                    };
                }
                else
                {
                    if (activeRecord.CheckOut.HasValue)
                        throw new Exception("Already checked out");

                    if (actionTime <= activeRecord.CheckIn)
                        throw new Exception("Check-out time must be after check-in");

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
                        Action = "check-out",
                        Message = $"Checked out from {application.Project.Title}",
                        HoursWorked = activeRecord.Hours,
                        TotalHours = account.Hour,
                        CheckInTime = activeRecord.CheckIn,
                        CheckOutTime = activeRecord.CheckOut
                    };
                }
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

    }
}