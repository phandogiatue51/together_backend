using Together.Models;

namespace Together.DTOs.Certi
{
    public class ViewCertiDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string CertificateName { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; } 

        public string? IssuingOrganization { get; set; }
        public string? CertificateNumber { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        //public CertificateStatus Status { get; set; }
        //public string? StatusName { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime? VerifiedAt { get; set; }
        //public int? VerifiedByAdminId { get; set; }
    }
}