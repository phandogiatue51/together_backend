using System.ComponentModel.DataAnnotations;
using Together.Models;

namespace Together.DTOs.Pro
{
    public class UpdateProjectDto : CreateProjectDto
    {
        [Required]
        public ProjectStatus Status { get; set; }
    }
}
