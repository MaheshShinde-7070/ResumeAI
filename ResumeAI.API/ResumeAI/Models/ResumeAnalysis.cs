namespace ResumeAI.API.Models;

public class ResumeAnalysis
{
    public int Id { get; set; }

    public int ResumeId { get; set; }

    public int OverallScore { get; set; }

    public string Skills { get; set; } = string.Empty;

    public string MissingSkills { get; set; } = string.Empty;

    public string Suggestions { get; set; } = string.Empty;

    public DateTime AnalyzedAt { get; set; }

    public Resume Resume { get; set; } = null!;
}