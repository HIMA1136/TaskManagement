namespace TaskManagement.Application.Features.Auth.DTOs;

public sealed record AuthResponse(
    string UserId,
    string Email,
    string Token,
    DateTime ExpiresAt);
