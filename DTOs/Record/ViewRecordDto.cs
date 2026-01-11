namespace Together.DTOs.Record
{
    public class ViewRecordDto
    {
        public int RecordId { get; set; }
        public int VolunteerApplicationId { get; set; }
        public DateTime? CheckIn { get; set; } = null;
        public DateTime? CheckOut { get; set; } = null;
        public decimal Hours { get; set; } = 0.00m;
    }
}