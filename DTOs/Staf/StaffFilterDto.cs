using Together.Models;

namespace Together.DTOs.Staf
{
    public class StaffFilterDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? OrganizationId { get; set; }
        public StaffRole? Role { get; set; }
        public DateTime? JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public bool? IsActive { get; set; }
    }
}
