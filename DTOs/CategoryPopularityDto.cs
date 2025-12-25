namespace Together.DTOs
{
    public class CategoryPopularityDto : ViewCateDto
    {
        public int ProjectCount { get; set; }
        public string? LatestProject { get; set; }
    }
}
