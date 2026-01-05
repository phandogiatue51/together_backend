using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public class VolunteerHour

    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }

        [Required]
        public int VolunteerApplicationId { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Hours { get; set; } = 0.00m;

        public DateTime? CheckIn { get; set; } = null;
        public DateTime? CheckOut { get; set; } = null;

        [ForeignKey(nameof(VolunteerApplicationId))]
        public virtual VolunteerApplication VolunteerApplication { get; set; }
    }
}