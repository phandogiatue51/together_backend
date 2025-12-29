using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Together.Models
{
    public enum BlogStatus
    {
        Draft,
        Pending,
        Published,
        Archived
    }

    public class BlogPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        [Required]
        public int AuthorId { get; set; }

        public int? OrganizationId { get; set; } 

        [ForeignKey("AuthorId")]
        public virtual Account Author { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization? Organization { get; set; } 

        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public BlogStatus Status { get; set; } = BlogStatus.Draft;
    }
}