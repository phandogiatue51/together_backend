using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public class ProjectCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;
        
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;
    }
}