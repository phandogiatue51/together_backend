using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Code { get; set; } 

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Icon { get; set; } 

        [MaxLength(50)]
        public string? Color { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<ProjectCategory> ProjectCategories { get; set; } = new List<ProjectCategory>();
    }
}