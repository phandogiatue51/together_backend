using Together.Models;

namespace Together.DTOs.Staf
{
    public class ViewStaffDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public int AccountId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public StaffRole Role { get; set; }
        public string? StaffRole { get; set; }
        public bool? IsActive { get; set; }
        public DateTime JoinedDate { get; set; }
        public DateTime? LeftDate { get; set; }
    }
}
