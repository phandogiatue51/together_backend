namespace Together.DTOs.Blog
{
    public class BlogFilterDto
    {
        public string? Title { get; set; }
        public int? AuthorId { get; set; }
        public int? OrganizationId { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool? Status { get; set; }
    }
}
