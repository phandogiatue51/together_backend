using System.ComponentModel.DataAnnotations;
using Together.Models;

namespace Together.DTOs.Organ
{
    public class CreateOrganDto
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public OrganizationType Type { get; set; }
        public IFormFile? ImageFile { get; set; }
        [Required]
        public string? Website { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Address { get; set; }
    }
}
