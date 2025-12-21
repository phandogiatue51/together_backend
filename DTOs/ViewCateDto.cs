using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Together.Models;

namespace Together.DTOs
{
    public class ViewCateDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Code { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

    }
}