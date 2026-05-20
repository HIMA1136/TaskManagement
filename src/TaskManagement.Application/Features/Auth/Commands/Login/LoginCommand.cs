using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Features.Auth.DTOs;

namespace TaskManagement.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<AuthResponse>;
