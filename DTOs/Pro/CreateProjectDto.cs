using System.ComponentModel.DataAnnotations;
using Together.Models;

namespace Together.DTOs.Pro
{
    public class CreateProjectDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ProjectType Type { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public IFormFile? ImageUrl { get; set; }

        [Required]
        [Range(1, 1000)]
        public int RequiredVolunteers { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}