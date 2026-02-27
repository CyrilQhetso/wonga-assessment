namespace AuthService.Models;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password
);

public record LoginRequest(
    string Email,
    string Password
);

public record UserDetailsResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime CreatedAt
);

public record AuthResponse(
    string Token,
    UserDetailsResponse User
);