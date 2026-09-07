using Microsoft.EntityFrameworkCore;
using ResumeAI.API.Models;

namespace ResumeAI.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Resume> Resumes { get; set; }

    public DbSet<ResumeAnalysis> ResumeAnalyses { get; set; }

    public DbSet<JobMatch> JobMatches { get; set; }
}