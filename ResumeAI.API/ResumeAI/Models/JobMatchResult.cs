namespace ResumeAI.API.Models;

public class JobMatchResult
{
    public int MatchPercentage { get; set; }

    public List<string> MatchedSkills { get; set; } = new();

    public List<string> MissingSkills { get; set; } = new();

    public List<string> Suggestions { get; set; } = new();
}