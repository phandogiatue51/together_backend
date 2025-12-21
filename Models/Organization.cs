using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public enum OrganizationType
    {
        NGO,          
        Corporate,    
        School,        
        Community,    
        Government      
    }

    public class Organization
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public OrganizationType Type { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Url]
        public string? Website { get; set; }

        public string? LogoUrl { get; set; }

        [Required]
        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
        public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
    }
}