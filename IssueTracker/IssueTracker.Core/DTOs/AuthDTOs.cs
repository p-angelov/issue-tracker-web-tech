using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Core.DTOs;

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);

public record RegisterRequest(
    [Required] 
    [MaxLength(50)] 
    string Username,
    
    [Required] 
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    string Password,
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    string Email,
    
    string? Role = "User"
);

public record AuthResponse(
    bool Success,
    string Token,
    string Username,
    string Email,
    string Role,
    DateTime Expiration
);

public record ErrorResponse(
    bool Success,
    string Message
); 