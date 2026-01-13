using System.ComponentModel.DataAnnotations;

namespace Together.DTOs.Record
{
    public class AttendanceDto
    {
        public class GenerateCodeRequestDto
        {
            [Required]
            public int ProjectId { get; set; }

            [Required]
            public string ActionType { get; set; } = "checkin";
        }

        public class GenerateCodeResponseDto
        {
            public string Code { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public string ProjectName { get; set; } = string.Empty;
            public string ActionType { get; set; } = string.Empty;
        }

        public class VerifyCodeRequestDto
        {
            [Required]
            [StringLength(6, MinimumLength = 6)]
            public string Code { get; set; } = string.Empty;
            public int AccountId { get; set; }
        }

        public class VerifyCodeResponseDto
        {
            public string Action { get; set; } = string.Empty; // "checkin" or "checkout"
            public string Message { get; set; } = string.Empty;
            public DateTime Time { get; set; }
            public int ProjectId { get; set; }
            public string ProjectName { get; set; } = string.Empty;
            public decimal? HoursWorked { get; set; }
            public decimal? TotalHours { get; set; }
        }

        public class AttendanceCodeData
        {
            public string Code { get; set; } = string.Empty;
            public int ProjectId { get; set; }
            public string ActionType { get; set; } = string.Empty; // "checkin" or "checkout"
            public DateTime ExpiresAt { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
