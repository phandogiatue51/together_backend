using System.ComponentModel.DataAnnotations;
using Together.Models;

namespace Together.DTOs.Organ
{
    public class CreateOrganWithManagerDto : CreateOrganDto
    {
        [Required]
        public CreateManagerDto Manager { get; set; } = null!;
    }

    public class CreateManagerDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public bool? IsFemale { get; set; }

        public StaffRole Role { get; set; } = StaffRole.Manager;
    }
}