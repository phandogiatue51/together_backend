using Together.Models;

namespace Together.DTOs.User
{
    public class UserFilterDto
    {
        public AccountRole? Role { get; set; }
        public AccountStatus? Status { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}