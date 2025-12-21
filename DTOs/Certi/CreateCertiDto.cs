using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.DTOs.Certi
{
    public class CreateCertiDto
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [MaxLength(200)]
        public string CertificateName { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [MaxLength(100)]
        public string? IssuingOrganization { get; set; }

        [MaxLength(50)]
        public string? CertificateNumber { get; set; }

        public DateOnly? IssueDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public IFormFile ImageUrl { get; set; }
    }
}