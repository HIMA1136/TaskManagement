using TaskManagement.Application.Common.Abstractions;
using TaskManagement.Application.Features.Projects.DTOs;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string Name,
    string? Description) : ICommand<ProjectSummaryDto>;
