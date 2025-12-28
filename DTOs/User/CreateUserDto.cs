using System.ComponentModel.DataAnnotations;

namespace Together.DTOs.User
{
    public class CreateUserDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public bool? IsFemale { get; set; }
        public IFormFile? ImageUrl { get; set; } = null;
    }
}
