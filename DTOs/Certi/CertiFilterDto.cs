using Together.Models;

namespace Together.DTOs.Certi
{
    public class CertiFilterDto
    {
        public int? AccountId { get; set; }
        public int? CategoryId { get; set; }
        public string? CertificateName { get; set; }
        public CertificateStatus? Status { get; set; }
    }
}