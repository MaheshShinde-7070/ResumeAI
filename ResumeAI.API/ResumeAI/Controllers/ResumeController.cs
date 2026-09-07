using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeAI.API.Data;
using ResumeAI.API.DTOs;
using ResumeAI.API.Interfaces;
using ResumeAI.API.Models;
using System.Security.Claims;
using UglyToad.PdfPig;

namespace ResumeAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResumeController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAIService _aiService;
    public ResumeController(AppDbContext context,IAIService aiService)
    {
        _context = context;
        _aiService = aiService;
    }
    [HttpPost("upload")]
    public async Task<IActionResult> UploadResume(IFormFile file)
    {
        

        if (file == null || file.Length == 0)
        {
            return BadRequest("Please upload a file.");
        }

        if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
        {
            return BadRequest("Only PDF files are allowed.");
        }

        // Create unique file name
        var fileName = Guid.NewGuid().ToString() + ".pdf";

        // Path to Uploads folder
        var uploadPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads"
        );

        // Create folder if it doesn't exist
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        // Complete file path
        var filePath = Path.Combine(uploadPath, fileName);
        var userId = int.Parse(
    User.FindFirst(ClaimTypes.NameIdentifier)!.Value
);
        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        string extractedText = string.Empty;

        using (var pdf = PdfDocument.Open(filePath))
        {
            foreach (var page in pdf.GetPages())
            {
                extractedText += page.Text;
            }
        }

        var resume = new Resume
        {
            UserId = userId,
            FileName = file.FileName,
            FilePath = filePath,
            ExtractedText = extractedText,
            UploadedAt = DateTime.UtcNow
        };

        _context.Resumes.Add(resume);
        await _context.SaveChangesAsync();

        
        return Ok(new
        {
            Message = "Resume uploaded successfully.",
            FileName = file.FileName
        });


    }

    [HttpPost("analyze-job")]
    public async Task<IActionResult> AnalyzeJob(
    JobMatchRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.JobDescription))
        {
            return BadRequest("Job description is required.");
        }

        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var resume = await _context.Resumes
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.UploadedAt)
            .FirstOrDefaultAsync();

        if (resume == null)
        {
            return NotFound("Please upload a resume first.");
        }

        var result = await _aiService.AnalyzeJobMatchAsync(
            resume.ExtractedText,
            request.JobDescription
        );

        return Ok(result);
    }

}