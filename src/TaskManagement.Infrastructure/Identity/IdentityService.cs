using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Common.Errors;

namespace TaskManagement.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : IIdentityService
{
    public async Task<Result<(string UserId, string Email, IList<string> Roles)>> CreateUserAsync(
        string email,
        string password,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return Error.Conflict("Identity.EmailAlreadyExists", $"A user with email '{email}' already exists.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Error.Failure("Identity.CreateFailed", errors);
        }

        var roles = await userManager.GetRolesAsync(user);
        return (user.Id, user.Email!, roles);
    }

    public async Task<Result<(string UserId, string Email, IList<string> Roles)>> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return Error.Unauthorized("Identity.InvalidCredentials", "Invalid email or password.");

        var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Error.Unauthorized("Identity.InvalidCredentials", "Invalid email or password.");

        var roles = await userManager.GetRolesAsync(user);
        return (user.Id, user.Email!, roles);
    }
}
