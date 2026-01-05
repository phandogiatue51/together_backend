using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public enum AccountRole
    {
        Volunteer,  
        Staff,       
        Admin       
    }

    public enum AccountStatus
    {
        Active,
        Inactive
    }

    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; } 

        public string? ProfileImageUrl { get; set; }

        public bool? IsFemale { get; set; }

        public DateOnly? DateOfBirth { get; set; }
        public decimal? Hour { get; set; } = 0.00m;

        [Required]
        public AccountRole Role { get; set; } = AccountRole.Volunteer;

        [Required]
        public AccountStatus Status { get; set; } = AccountStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
        public virtual ICollection<VolunteerApplication> VolunteerApplications { get; set; } = new List<VolunteerApplication>();
        public virtual ICollection<VolunteerHour> VolunteerHours { get; set; } = new List<VolunteerHour>();
        public virtual ICollection<Staff> OrganizationStaff { get; set; } = new List<Staff>();
    }
}