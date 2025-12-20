namespace Together.DTOs.Blog
{
    public class UpdateBlogDto
    {
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Excerpt { get; set; }

        public IFormFile? FeaturedImageFile { get; set; }
        public IFormFile? ImageFile1 { get; set; }
        public IFormFile? ImageFile2 { get; set; }
        public IFormFile? ImageFile3 { get; set; }
        public IFormFile? ImageFile4 { get; set; }
        public IFormFile? ImageFile5 { get; set; }

        public bool? RemoveFeaturedImage { get; set; }
        public bool? RemoveImage1 { get; set; }
        public bool? RemoveImage2 { get; set; }
        public bool? RemoveImage3 { get; set; }
        public bool? RemoveImage4 { get; set; }
        public bool? RemoveImage5 { get; set; }

        public string? Paragraph1 { get; set; }
        public string? Paragraph2 { get; set; }
        public string? Paragraph3 { get; set; }
        public string? Paragraph4 { get; set; }
        public string? Paragraph5 { get; set; }

        public bool? Status { get; set; }
    }
}