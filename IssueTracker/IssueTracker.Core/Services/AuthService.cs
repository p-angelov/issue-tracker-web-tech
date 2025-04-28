using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IssueTracker.Core.DTOs;
using IssueTracker.Core.Interfaces;
using IssueTracker.Core.Models;
using IssueTracker.Core.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IssueTracker.Core.Services;

public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly List<User> _users = new(); // Temporary in-memory store; replace with your database

    public AuthService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        
        // Add a default admin user for testing
        _users.Add(new User
        {
            Id = Guid.NewGuid().GetHashCode(),
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Email = "admin@example.com",
            Role = "Admin"
        });
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // In a real implementation, this would be a database lookup
        var user = _users.FirstOrDefault(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResponse(false, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue);
        }

        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        return new AuthResponse(true, token, user.Username, user.Email, user.Role, expiration);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        if (_users.Any(u => u.Username == request.Username || u.Email == request.Email))
        {
            return new AuthResponse(false, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue);
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid().GetHashCode(),
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Email = request.Email,
            Role = request.Role ?? "User" // Default to User role if not specified
        };

        _users.Add(user);

        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        return new AuthResponse(true, token, user.Username, user.Email, user.Role, expiration);
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
} 