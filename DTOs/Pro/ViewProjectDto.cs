using Together.Models;

namespace Together.DTOs.Pro
{
    public class ViewProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProjectType Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public int OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public ProjectStatus Status { get; set; }
        public int RequiredVolunteers { get; set; }
        public int CurrentVolunteers { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<ProjectCategoryDto> Categories { get; set; } = new List<ProjectCategoryDto>();

    }

    public class ProjectCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryIcon { get; set; }
        public string? CategoryColor { get; set; }
    }
}