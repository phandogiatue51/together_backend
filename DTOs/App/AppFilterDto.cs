using Together.Models;

namespace Together.DTOs.App
{
    public class AppFilterDto
    {
        public int? OrganizationId { get; set; }
        public int? ProjectId { get; set; }
        public int? VolunteerId { get; set; }
        public ApplicationStatus? Status { get; set; }
    }
}