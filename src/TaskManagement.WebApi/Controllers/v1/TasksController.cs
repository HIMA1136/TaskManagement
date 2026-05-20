using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Tasks.Commands.ChangeTaskStatus;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagement.Application.Features.Tasks.Queries.GetTaskById;
using TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;
using TaskManagement.Domain.Common.Errors;
using TaskManagement.Domain.Tasks;

namespace TaskManagement.WebApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/tasks")]
[Authorize]
public sealed class TasksController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetTasksByProjectQuery(projectId, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid projectId, Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetTaskByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result.Error!);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var command = new CreateTaskCommand(
            projectId,
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.AssigneeId);

        var result = await sender.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { projectId, id = result.Value!.Id }, result.Value)
            : HandleFailure(result.Error!);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid projectId, Guid id, [FromBody] UpdateTaskRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdateTaskCommand(id, projectId, request.Title, request.Description, request.Priority, request.DueDate),
            ct);

        return result.IsSuccess ? NoContent() : HandleFailure(result.Error!);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid projectId, Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteTaskCommand(id, projectId), ct);
        return result.IsSuccess ? NoContent() : HandleFailure(result.Error!);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(Guid projectId, Guid id, [FromBody] ChangeTaskStatusRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ChangeTaskStatusCommand(id, projectId, request.NewStatus), ct);
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

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    string AssigneeId);

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate);

public sealed record ChangeTaskStatusRequest(TaskManagement.Domain.Tasks.TaskStatus NewStatus);
