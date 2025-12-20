using Together.Models;

namespace Together.DTOs.Pro
{
    public class ProjectDetailDto : ViewProjectDto
    {
        public List<ViewFormDto> Forms { get; set; } = new();
        public int AvailableSlots => RequiredVolunteers - CurrentVolunteers;
    }

    public class ViewFormDto
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int ResponseCount { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
    }
}