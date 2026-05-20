namespace TaskManagement.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(string userId, string email, IList<string> roles);
    DateTime GetExpirationUtc();
}
