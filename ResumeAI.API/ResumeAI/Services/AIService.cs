using ResumeAI.API.Interfaces;
using System.Net.Http.Json;
using ResumeAI.API.Models;
using System.Text.Json;
namespace ResumeAI.API.Services;

public class AIService : IAIService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AIService(
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

   
    public async Task<JobMatchResult> AnalyzeJobMatchAsync(
    string resumeText,
    string jobDescription)
    {
        var apiKey = _configuration["Groq:ApiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Groq API key not found.");
        }

        _httpClient.DefaultRequestHeaders.Clear();

        _httpClient.DefaultRequestHeaders.Add(
            "Authorization",
            $"Bearer {apiKey}"
        );

        var requestBody = new
        {
            model = "openai/gpt-oss-20b",

            messages = new[]
            {
            new
            {
                role = "user",

                content = $"""
Compare this resume with the job description.

Return ONLY valid JSON.
Do not include markdown, ```json, or any explanation outside the JSON.

Use exactly these properties:

matchPercentage: number from 0 to 100
matchedSkills: skills required by the job that are present in the resume
missingSkills: important skills required by the job that are missing from the resume
suggestions: useful recommendations to improve the resume for this specific job

Resume:
{resumeText}

Job Description:
{jobDescription}
"""
            }
        }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "https://api.groq.com/openai/v1/chat/completions",
            requestBody
        );

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<GroqResponse>();

        var aiJson = result?.Choices?[0]?.Message?.Content;

        Console.WriteLine("========== JOB MATCH AI RESPONSE ==========");
        Console.WriteLine(aiJson);
        Console.WriteLine("===========================================");

        if (string.IsNullOrEmpty(aiJson))
        {
            throw new Exception("No AI response received.");
        }
        var analysis = JsonSerializer.Deserialize<JobMatchResult>(
            aiJson,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        if (analysis == null)
        {
            throw new Exception("Unable to process AI response.");
        }

        return analysis;
    }
    public class GroqResponse
    {
        public List<GroqChoice>? Choices { get; set; }
    }

    public class GroqChoice
    {
        public GroqMessage? Message { get; set; }
    }

    public class GroqMessage
    {
        public string? Content { get; set; }
    }
}