using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public enum StaffRole
    {
        Member,
        Manager
    }

    public class Staff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrganizationId { get; set; }
        public int AccountId { get; set; }

        public StaffRole Role { get; set; } = StaffRole.Member;

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        [ForeignKey("AccountId")]
        public virtual Account StaffAccount { get; set; }
    }
}
