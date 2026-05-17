using Microsoft.AspNetCore.Mvc;
using CineReserveAPI.Data;
using CineReserveAPI.DTOs;
using CineReserveAPI.Helpers;
using CineReserveAPI.Models;

namespace CineReserveAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtHelper _jwt;

    public AuthController(AppDbContext db, JwtHelper jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (_db.Users.Any(u => u.Email == dto.Email))
            return BadRequest(new { message = "Email already registered" });

        // Limit Role to only "User" or "Admin"
        var role = dto.Role == "Admin" ? "Admin" : "User";

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { message = $"Registered as {role}" });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(new AuthResponseDto
        {
            Token = _jwt.GenerateToken(user),
            Username = user.Username,
            Role = user.Role,
            CreditBalance = user.CreditBalance
        });
    }
}