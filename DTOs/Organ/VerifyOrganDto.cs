using Together.Models;

namespace Together.DTOs.Organ
{
    public class VerifyOrganDto
    {
        public OrganzationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public int AdminId { get; set; }
    }
}
