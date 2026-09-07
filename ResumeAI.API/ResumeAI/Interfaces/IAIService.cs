using ResumeAI.API.Models;

namespace ResumeAI.API.Interfaces;

public interface IAIService
{
        Task<JobMatchResult> AnalyzeJobMatchAsync(
        string resumeText,
        string jobDescription
    );
}