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

        public ViewRecordDto MapToViewRecordDto(VolunteerHour hour)
        {
            return new ViewRecordDto
            {
                RecordId = hour.RecordId,
                VolunteerApplicationId = hour.VolunteerApplicationId,
                CheckIn = hour.CheckIn,
                CheckOut = hour.CheckOut,
                Hours = hour.Hours,
            };
        }
    }
}
