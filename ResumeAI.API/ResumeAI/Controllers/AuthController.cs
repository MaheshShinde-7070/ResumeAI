using Microsoft.AspNetCore.Mvc;
using ResumeAI.API.Data;
using ResumeAI.API.DTOs;
using ResumeAI.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ResumeAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(
        AppDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }


    // =========================
    // REGISTER
    // =========================

    [HttpPost("register")]
    public IActionResult Register(RegisterDto dto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest("Name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Password is required.");
        }


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


        return Ok(new
        {
            message = "User registered successfully."
        });
    }


    // =========================
    // LOGIN
    // =========================

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Password is required.");
        }


        // Find user by email
        var user = _context.Users
            .FirstOrDefault(u => u.Email == dto.Email);


        // User doesn't exist
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }


        // Verify password
        var passwordValid = BCrypt.Net.BCrypt.Verify(
            dto.Password,
            user.PasswordHash
        );


        // Password is incorrect
        if (!passwordValid)
        {
            return Unauthorized("Invalid email or password.");
        }


        // =========================
        // JWT CLAIMS
        // =========================

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            new Claim(
                ClaimTypes.Name,
                user.Name
            ),

            new Claim(
                ClaimTypes.Email,
                user.Email
            )
        };


        // =========================
        // JWT KEY
        // =========================

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );


        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );


        // =========================
        // CREATE TOKEN
        // =========================

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(
                    _configuration["Jwt:ExpiryMinutes"]
                )
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
}