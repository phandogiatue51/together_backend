using System.ComponentModel.DataAnnotations;

namespace Together.DTOs.Blog
{
    public class CreateBlogDto
    {
        [Required]
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Excerpt { get; set; }

        public IFormFile? ImageUrl1 { get; set; }
        public IFormFile? ImageUrl2 { get; set; }
        public IFormFile? ImageUrl3 { get; set; }
        public IFormFile? ImageUrl4 { get; set; }
        public IFormFile? ImageUrl5 { get; set; }

        [Required]
        public string? Paragraph1 { get; set; }
        public string? Paragraph2 { get; set; }
        public string? Paragraph3 { get; set; }
        public string? Paragraph4 { get; set; }
        public string? Paragraph5 { get; set; }

        public IFormFile? FeaturedImageUrl { get; set; }

        public int? AuthorId { get; set; }
        public int? OrganizationId { get; set; }
    }
}
