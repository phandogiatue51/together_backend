using Together.Models;

namespace Together.DTOs.Pro
{
    public class ProjectFilterDto
    {
        public string? Title { get; set; }
        public ProjectType? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public ProjectStatus? Status { get; set; }
        public int? RequiredVolunteers { get; set; }
        public int? CurrentVolunteers { get; set; }
        public DateTime? CreatedAt { get; set; }

        public List<int>? CategoryIds { get; set; } = new List<int>();
        public int? OrganizationId { get; set; }
    }

}
