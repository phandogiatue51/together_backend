using System.ComponentModel.DataAnnotations;

namespace Together.DTOs.App
{
    public class CreateAppDto
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int VolunteerId { get; set; }
        public string? RelevantExperience { get; set; }
        public int SelectedCertificateId { get; set; }
    }
}
