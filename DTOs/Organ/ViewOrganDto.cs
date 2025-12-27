using Together.Models;

namespace Together.DTOs.Organ
{
    public class ViewOrganDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public OrganizationType Type { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public OrganzationStatus Status { get; set; }
        public string? StatusName { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
