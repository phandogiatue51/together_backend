using System.ComponentModel.DataAnnotations;

namespace Together.DTOs.Record
{
    public class QrDto
    {
        public class QrActionDto
        {
            [Required]
            public string QrToken { get; set; } = string.Empty;

            public DateTime? ActionTime { get; set; } = DateTime.Now;
            [Required]
            public int AccountId { get; set; }
        }

        public class QrResponseDto
        {
            public string QrToken { get; set; }
            public string QrImageBase64 { get; set; }
            public DateTime ExpiresAt { get; set; }
            public string ProjectName { get; set; }
            public string ActionType { get; set; } 
        }
    }
}