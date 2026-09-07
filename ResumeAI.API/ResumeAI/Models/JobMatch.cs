namespace ResumeAI.API.Models;

public class JobMatch
{
    public int Id { get; set; }

    public int ResumeId { get; set; }

    public string JobDescription { get; set; } = string.Empty;

    public int MatchPercentage { get; set; }

    public string MissingSkills { get; set; } = string.Empty;

    public string Suggestions { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public Resume Resume { get; set; } = null!;
}