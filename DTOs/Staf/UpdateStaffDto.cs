using Together.Models;

namespace Together.DTOs.Staf
{
    public class UpdateStaffDto
    {
        public StaffRole? Role { get; set; }
        public bool? IsActive { get; set; }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public bool? IsFemale { get; set; }
    }
}
