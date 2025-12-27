using Together.Models;

namespace Together.DTOs.Blog
{
    public class ViewBlogDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Excerpt { get; set; }

        public string? ImageUrl1 { get; set; }
        public string? ImageUrl2 { get; set; }
        public string? ImageUrl3 { get; set; }
        public string? ImageUrl4 { get; set; }
        public string? ImageUrl5 { get; set; }

        public string? Paragraph1 { get; set; }
        public string? Paragraph2 { get; set; }
        public string? Paragraph3 { get; set; }
        public string? Paragraph4 { get; set; }
        public string? Paragraph5 { get; set; }

        public string? FeaturedImageUrl { get; set; }

        public int? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public int? OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public BlogStatus Status { get; set; }
        public string? StatusName { get; set; }
    }
}
