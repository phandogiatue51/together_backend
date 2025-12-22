using Together.Models;

namespace Together.DTOs.User
{
    public class ViewUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public bool IsFemale { get; set; }
        public AccountRole Role { get; set; }
        public AccountStatus Status { get; set; }
    }
}
