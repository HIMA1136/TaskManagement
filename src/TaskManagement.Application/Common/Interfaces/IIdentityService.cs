using TaskManagement.Domain.Common.Errors;

namespace TaskManagement.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<(string UserId, string Email, IList<string> Roles)>> CreateUserAsync(
        string email, string password, string firstName, string lastName,
        CancellationToken cancellationToken = default);

    Task<Result<(string UserId, string Email, IList<string> Roles)>> AuthenticateAsync(
        string email, string password,
        CancellationToken cancellationToken = default);
}
