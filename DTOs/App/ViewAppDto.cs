using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Together.DTOs.Certi;
using Together.Models;

namespace Together.DTOs.App
{
    public class ViewAppDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectTitle { get; set; }
        public int? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public int VolunteerId { get; set; }
        public string? VolunteerName { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? StatusName { get; set; }
        public string? RelevantExperience { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedByStaffId { get; set; }
        public string? RejectionReason { get; set; }

        public string? Feedback { get; set; }
        public List<VolunteerHourDto> VolunteerHours { get; set; } = new();
        public ViewCertiDto? SelectedCertificate { get; set; }
    }

    public class VolunteerHourDto { 
        public DateTime? CheckIn { get; set; } 
        public DateTime? CheckOut { get; set; } 
        public decimal Hours { get; set; } 
    }
}
