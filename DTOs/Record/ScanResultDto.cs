namespace Together.DTOs.Record
{
    public class ScanResultDto
    {
        public string Action { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime? Time { get; set; }
        public int? RecordId { get; set; }
        public decimal? HoursWorked { get; set; }
        public decimal? TotalHours { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}