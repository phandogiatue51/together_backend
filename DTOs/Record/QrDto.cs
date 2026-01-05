using System.ComponentModel.DataAnnotations;

namespace Together.DTOs.Record
{
    public class QrDto
    {
        public class QrActionDto
        {
            [Required]
            public string QrToken { get; set; } = string.Empty;

            public DateTime? ActionTime { get; set; }
        }

        public class GenerateQrDto
        {
            [Required]
            public int ProjectId { get; set; }

            public int? DurationHours { get; set; } = 24;
        }

        public class QrResponseDto
        {
            public string QrToken { get; set; } = string.Empty;
            public string QrImageBase64 { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public string ProjectName { get; set; } = string.Empty;
        }
    }
}