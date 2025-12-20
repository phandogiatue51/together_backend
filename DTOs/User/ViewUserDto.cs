using Together.Models;

namespace Together.DTOs.User
{
    public class ViewUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public AccountRole Role { get; set; }
        public AccountStatus Status { get; set; }
    }
}
