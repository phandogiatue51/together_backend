using System.ComponentModel.DataAnnotations;

namespace Together.DTOs.Organ
{
    public class CreateOrganDto
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        [Required]
        public string? Website { get; set; }
        [Required]
        public string? ContactEmail { get; set; }
        [Required]
        public string? ContactPhone { get; set; }
        [Required]
        public string? Address { get; set; }
    }
}
