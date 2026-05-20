using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Auth.DTOs;
using TaskManagement.Domain.Common.Errors;

namespace TaskManagement.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IIdentityService identityService,
    IJwtService jwtService)
    : ICommandHandler<RegisterCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            cancellationToken);

        if (result.IsFailure) return result.Error!;

        var (userId, email, roles) = result.Value!;
        var token = jwtService.GenerateToken(userId, email, roles);

        return new AuthResponse(userId, email, token, jwtService.GetExpirationUtc());
    }
}
