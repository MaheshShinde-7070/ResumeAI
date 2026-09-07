using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ResumeAI.API.Data;
using ResumeAI.API.DTOs;
using ResumeAI.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ResumeAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        // Check whether email already exists
        var existingUser = _context.Users
            .FirstOrDefault(u => u.Email == dto.Email);

        if (existingUser != null)
        {
            return BadRequest("Email already registered.");
        }

        // Create new user
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        // Save user
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new { message = "User registered successfully." });
    }
    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        // 1. Find user by email
        var user = _context.Users
            .FirstOrDefault(u => u.Email == dto.Email);

        // 2. User doesn't exist
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        // 3. Verify the entered password against the stored hash
        var passwordValid = BCrypt.Net.BCrypt.Verify(
            dto.Password,
            user.PasswordHash
        );

        // 4. Password is incorrect
        if (!passwordValid)
        {
            return Unauthorized("Invalid email or password.");
        }

        var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Name),
    new Claim(ClaimTypes.Email, user.Email)
};

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])
            ),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Ok(new LoginResponseDto
        {
            Token = tokenString
        });
    }
    [HttpGet("profile")]
    [Authorize]
    public IActionResult Profile()
    {
        return Ok("You are authenticated!");
    }
}