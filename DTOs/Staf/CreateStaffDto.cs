using System.ComponentModel.DataAnnotations;
using Together.DTOs.User;
using Together.Models;

namespace Together.DTOs.Staf
{
    public class CreateStaffDto
    {
        public CreateUserDto? NewAccount { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public StaffRole Role { get; set; }
    }
}
