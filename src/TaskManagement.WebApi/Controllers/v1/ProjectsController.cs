using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Projects.Commands.CreateProject;
using TaskManagement.Application.Features.Projects.Commands.DeleteProject;
using TaskManagement.Application.Features.Projects.Commands.UpdateProject;
using TaskManagement.Application.Features.Projects.Queries.GetAllProjects;
using TaskManagement.Application.Features.Projects.Queries.GetProjectById;
using TaskManagement.Domain.Common.Errors;

namespace TaskManagement.WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetAllProjectsQuery(page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetProjectByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProjectCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : HandleFailure(result.Error!);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateProjectCommand(id, request.Name, request.Description), ct);
        return result.IsSuccess ? NoContent() : HandleFailure(result.Error!);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteProjectCommand(id), ct);
        return result.IsSuccess ? NoContent() : HandleFailure(result.Error!);
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

public sealed record UpdateProjectRequest(string Name, string? Description);
