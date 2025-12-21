using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public enum StaffRole
    {
        Manager,       
        Coordinator,    
        Reviewer,       
        VolunteerManager 
    }

    public class Staff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public StaffRole Role { get; set; } = StaffRole.Coordinator;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; } = null!;

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;
    }
}