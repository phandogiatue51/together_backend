using Together.DTOs.Record;
using Together.DTOs.User;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class HourService
    {
        private readonly HourRepo _hourRepo;
        public HourService(HourRepo hourRepo)
        {
            _hourRepo = hourRepo;
        }

        public async Task<List<ViewRecordDto>> GetAllRecords()
        {
            var hours = await _hourRepo.GetAll();
            return hours.Select(MapToViewRecordDto).ToList();
        }

        public async Task<ViewRecordDto> GetRecordByIdAsync(int id)
        {
            var hour = await _hourRepo.GetByIdAsync(id);
            return MapToViewRecordDto(hour);
        }

        public async Task<List<ViewRecordDto>> GetRecordByProjectId(int id)
        {
            var hours = await _hourRepo.GetByProjectId(id);
            return hours.Select(MapToViewRecordDto).ToList();
        }

        public ViewRecordDto MapToViewRecordDto(VolunteerHour entity)
        {
            return new ViewRecordDto
            {
                RecordId = entity.RecordId,
                VolunteerApplicationId = entity.VolunteerApplicationId,
                CheckIn = entity.CheckIn,
                CheckOut = entity.CheckOut,
                Hours = entity.Hours,
                Volunteer = entity.VolunteerApplication?.Volunteer == null ? null : new VolunteerDto
                {
                    Id = entity.VolunteerApplication.Volunteer.Id,
                    Name = entity.VolunteerApplication.Volunteer.Name,
                    ProfileImageUrl = entity.VolunteerApplication.Volunteer.ProfileImageUrl,
                    Email = entity.VolunteerApplication.Volunteer.Email,
                    PhoneNumber = entity.VolunteerApplication.Volunteer.PhoneNumber
                }
            };
        }
    }
}
