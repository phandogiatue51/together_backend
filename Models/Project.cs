using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public enum ProjectStatus
    {
        Draft,         
        Planning,      
        Recruiting,    
        Active,         
        Completed,     
        Cancelled     
    }

    public enum ProjectType
    {
        Teaching,          
        MedicalSupport,    
        Infrastructure,     
        Event,             
        Training,          
        Counseling,        
        Other              
    }

    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ProjectType Type { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public ProjectStatus Status { get; set; } = ProjectStatus.Draft;

        [Required]
        [Range(1, 1000)]
        public int RequiredVolunteers { get; set; }

        public int CurrentVolunteers { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; } = null!;

        public virtual ICollection<VolunteerApplication> VolunteerApplications { get; set; } = new List<VolunteerApplication>();
        public virtual ICollection<ProjectCategory> Categories { get; set; } = new List<ProjectCategory>();
    }
}