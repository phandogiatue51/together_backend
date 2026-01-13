namespace Together.DTOs.Record
{
    public class ViewRecordDto
    {
        public int RecordId { get; set; }
        public int VolunteerApplicationId { get; set; }

        public VolunteerDto Volunteer { get; set; }

        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public decimal Hours { get; set; }
    }

    public class VolunteerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProfileImageUrl { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}