using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Auth.Commands.Login;
using TaskManagement.Application.Features.Auth.Commands.Register;
using TaskManagement.Domain.Common.Errors;

namespace TaskManagement.WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : HandleFailure(result.Error!);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
    }

    private IActionResult HandleFailure(Error error) => error.Type switch
    {
        ErrorType.NotFound => NotFound(ToProblemDetails(error, StatusCodes.Status404NotFound)),
        ErrorType.Unauthorized => Unauthorized(ToProblemDetails(error, StatusCodes.Status401Unauthorized)),
        ErrorType.Conflict => Conflict(ToProblemDetails(error, StatusCodes.Status409Conflict)),
        ErrorType.Validation => BadRequest(ToProblemDetails(error, StatusCodes.Status400BadRequest)),
        _ => StatusCode(StatusCodes.Status500InternalServerError, ToProblemDetails(error, StatusCodes.Status500InternalServerError))
    };

    private ProblemDetails ToProblemDetails(Error error, int statusCode) => new()
    {
        Title = error.Code,
        Detail = error.Description,
        Status = statusCode,
        Instance = HttpContext.Request.Path
    };
}
