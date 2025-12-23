using Together.DTOs.User;
using Together.Models;

namespace Together.DTOs.Staf
{
    public class UpdateStaffDto : UpdateUserDto
    {
        public StaffRole? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
