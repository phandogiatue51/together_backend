using Together.Models;

namespace Together.DTOs.Organ
{
    public class OrganFilterDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public OrganizationType? Type { get; set; }
        public OrganzationStatus? Status { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}