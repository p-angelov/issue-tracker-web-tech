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

public class AuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUsersRepository _usersRepository;

    public AuthService(IOptions<JwtSettings> jwtSettings, IUsersRepository usersRepository)
    {
        _jwtSettings = jwtSettings.Value;
        _usersRepository = usersRepository;
        
        // Seed default admin user if not exists
        SeedDefaultAdminUser().GetAwaiter().GetResult();
    }

    private async Task SeedDefaultAdminUser()
    {
        var existingAdmin = await _usersRepository.GetByUsername("admin");
        if (existingAdmin == null)
        {
            await _usersRepository.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Email = "admin@example.com",
                Role = "Admin"
            });
        }
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        User? user = await _usersRepository.GetByUsername(request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new AuthResponse(false, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue);
        }

        // Generate JWT token
        string token = GenerateJwtToken(user);
        DateTime expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        return new AuthResponse(true, token, user.Username, user.Email, user.Role, expiration);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        if (await _usersRepository.GetByUsername(request.Username) != null || 
            await _usersRepository.GetByEmail(request.Email) != null)
        {
            return new AuthResponse(false, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue);
        }

        // Create new user
        User user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Email = request.Email,
            Role = request.Role ?? "User" // Default to User role if not specified
        };

        await _usersRepository.Add(user);

        // Generate JWT token
        string token = GenerateJwtToken(user);
        DateTime expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        return new AuthResponse(true, token, user.Username, user.Email, user.Role, expiration);
    }

    private string GenerateJwtToken(User user)
    {
        byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        
        List<Claim> claims =
        [
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("UserId", user.Id.ToString())
        ];

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
} 