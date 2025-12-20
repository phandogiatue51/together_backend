using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public enum AccountRole
    {
        User,
        Admin,
        Staff
    }

    public enum AccountStatus
    {
        Active,
        Inactive,
        Banned,
    }

    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public AccountRole? Role { get; set; }
        public AccountStatus? Status { get; set; } = AccountStatus.Active;

    }
}
