namespace Together.DTOs.Pro
{
    public class ViewProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public int RequiredVolunteers { get; set; }
        public int CurrentVolunteers { get; set; } = 0;
    }
}
