using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public enum ApplicationStatus
    {
        Pending,       
        Approved,       
        Rejected,      
        Withdrawn,     
        Completed     
    }

    public class VolunteerApplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int VolunteerId { get; set; }

        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        [MaxLength(1000)]
        public string? RelevantExperience { get; set; } 

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedByStaffId { get; set; } 

        [MaxLength(500)]
        public string? RejectionReason { get; set; } 

        public int? Rating { get; set; } 

        [MaxLength(1000)]
        public string? Feedback { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;

        [ForeignKey("VolunteerId")]
        public virtual Account Volunteer { get; set; } = null!;

        [ForeignKey("ReviewedByStaffId")]
        public virtual Staff? ReviewedByStaff { get; set; }
        public virtual ICollection<Certificate> SelectedCertificates { get; set; } = new List<Certificate>();
    }
}