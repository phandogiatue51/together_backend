using Together.Models;

namespace Together.Helpers
{
    public class CalculateScore
    {
        public double CalculateMatchScore(Project project, List<Certificate> volunteerCerts, List<int> volunteerCategoryIds)
        {
            var projectCategoryIds = project.Categories
                .Select(pc => pc.CategoryId)
                .Distinct()
                .ToList();

            if (!projectCategoryIds.Any())
                return 0;

            var matchingCategoryIds = projectCategoryIds
                .Intersect(volunteerCategoryIds)
                .ToList();

            double categoryMatchPercentage = (double)matchingCategoryIds.Count / projectCategoryIds.Count * 100;

            double certificateBonus = 0;
            foreach (var categoryId in matchingCategoryIds)
            {
                var certsInCategory = volunteerCerts.Count(c => c.CategoryId == categoryId);
                if (certsInCategory > 1)
                    certificateBonus += 5;
            }

            var totalScore = Math.Min(100, categoryMatchPercentage + certificateBonus);

            return Math.Round(totalScore, 1);
        }

        public List<string> GetMatchingCategories(Project project, List<int> volunteerCategoryIds)
        {
            var matchingCategories = project.Categories
                .Where(pc => volunteerCategoryIds.Contains(pc.CategoryId))
                .Select(pc => pc.Category?.Name ?? "Unknown")
                .Distinct()
                .ToList();

            return matchingCategories;
        }

        public string GenerateMatchExplanation(Project project, List<string> matchingSkills, double matchScore)
        {
            if (matchingSkills.Count == 0)
                return "No skill match found";

            if (matchScore >= 80)
                return $"Perfect match! Your skills in {string.Join(", ", matchingSkills)} are exactly what this project needs.";

            if (matchScore >= 50)
                return $"Good match! Your {string.Join(", ", matchingSkills)} skills are relevant for this project.";

            return $"Partial match: Your {matchingSkills.First()} skill matches one of the project requirements.";
        }
    }
}
