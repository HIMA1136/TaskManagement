using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Auth.DTOs;
using TaskManagement.Domain.Common.Errors;

namespace TaskManagement.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IIdentityService identityService,
    IJwtService jwtService)
    : ICommandHandler<LoginCommand, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await identityService.AuthenticateAsync(request.Email, request.Password, cancellationToken);

        if (result.IsFailure) return result.Error!;

        var (userId, email, roles) = result.Value!;
        var token = jwtService.GenerateToken(userId, email, roles);

        return new AuthResponse(userId, email, token, jwtService.GetExpirationUtc());
    }
}
