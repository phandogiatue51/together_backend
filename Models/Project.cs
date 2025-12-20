using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }

        public int OrganizationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

        public int RequiredVolunteers { get; set; }
        public int CurrentVolunteers { get; set; } = 0;

        public virtual ICollection<Form> Forms { get; set; }
    }

    public enum ProjectStatus
    {
        Planning,
        Active,
        Completed,
        Cancelled
    }
}
