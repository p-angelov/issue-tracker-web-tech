using IssueTracker.Core.DTOs;
using IssueTracker.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        AuthResponse result = await authService.LoginAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return Unauthorized(new ErrorResponse(false, "Invalid username or password"));
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        AuthResponse result = await authService.RegisterAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(new ErrorResponse(false, "Registration failed"));
    }
    
    [HttpGet("test-auth")]
    [Authorize]
    public IActionResult TestAuth() => Ok(new { message = "You are authorized", user = User.Identity?.Name });

    [HttpGet("admin-only")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly() => Ok(new { message = "You are an admin" });
}