namespace Together.DTOs.App
{
    public class AppFilterDto
    {
        public int? OrganizationId { get; set; }
        public int? ProjectId { get; set; }
        public int? VolunteerId { get; set; }
        public string? Status { get; set; }
    }
}