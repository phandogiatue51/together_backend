namespace Together.DTOs.Pro
{
    public class ViewMatchedProjectDto : ViewProjectDto
    {
        public double MatchPercentage { get; set; }
        public int MatchingSkillCount { get; set; }
        public List<string> MatchingSkills { get; set; } = new List<string>();
        public string MatchExplanation { get; set; } = string.Empty;
    }
}
