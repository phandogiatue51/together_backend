using Together.Models;

namespace Together.DTOs.App
{
    public class ReviewAppDto
    {
        public ApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public int? ReviewedByStaffId { get; set; }
    }
}