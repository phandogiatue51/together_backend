namespace Together.DTOs.Pro
{
    public class CreateProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int OrganizationId { get; set; }
        public int RequiredVolunteers { get; set; }
    }
}