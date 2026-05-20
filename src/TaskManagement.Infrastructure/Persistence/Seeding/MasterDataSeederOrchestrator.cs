using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Domain.Projects;
using TaskManagement.Domain.Tasks;
using TaskManagement.Infrastructure.Identity;
using TaskManagement.Infrastructure.Settings;
using TaskStatus = TaskManagement.Domain.Tasks.TaskStatus;

namespace TaskManagement.Infrastructure.Persistence.Seeding;

public sealed class MasterDataSeederOrchestrator(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IProjectRepository projectRepository,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork,
    IOptions<SeedDataSettings> options,
    ILogger<MasterDataSeederOrchestrator> logger)
{
    private readonly SeedDataSettings _settings = options.Value;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
        {
            logger.LogInformation("Master data seeding is disabled.");
            return;
        }

        if (await context.Projects.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Master data already exists. Skipping seed.");
            return;
        }

        var admin = await EnsureUserAsync("admin@taskmanagement.local", "System", "Admin");
        var productOwner = await EnsureUserAsync("owner@taskmanagement.local", "Product", "Owner");
        var engineer = await EnsureUserAsync("engineer@taskmanagement.local", "Delivery", "Engineer");
        var qa = await EnsureUserAsync("qa@taskmanagement.local", "Quality", "Analyst");

        await SeedProjectAsync(
            "Workspace Modernization",
            "Refresh the delivery workspace, session flow, and reporting experience.",
            productOwner.Id,
            [
                ("Design Jira-inspired board", "Build an interactive board with live task status movement.", TaskPriority.High, TaskStatus.InProgress, engineer.Id, 5),
                ("Session timeout validation", "Make session lifetime configurable and visible to users.", TaskPriority.High, TaskStatus.Todo, admin.Id, 2),
                ("Regression checklist", "Validate seeded master data and main user journeys.", TaskPriority.Medium, TaskStatus.Done, qa.Id, -1)
            ],
            cancellationToken);

        await SeedProjectAsync(
            "Client Onboarding",
            "Prepare the first-run experience with realistic sample work for demos and testing.",
            admin.Id,
            [
                ("Seed starter projects", "Create realistic projects, owners, and due dates for walkthroughs.", TaskPriority.High, TaskStatus.Done, admin.Id, -2),
                ("Import delivery templates", "Prepare reusable templates for backlog grooming and delivery tracking.", TaskPriority.Medium, TaskStatus.InProgress, productOwner.Id, 4),
                ("Review access requests", "Confirm every seeded user can authenticate successfully.", TaskPriority.Low, TaskStatus.Todo, qa.Id, 1)
            ],
            cancellationToken);

        await SeedProjectAsync(
            "Support Queue Stabilization",
            "Track support issues with clear priority and accountability across the team.",
            engineer.Id,
            [
                ("Resolve stale tickets", "Clear blocked work and move owners onto active follow-up.", TaskPriority.High, TaskStatus.InProgress, engineer.Id, 3),
                ("Triage bug patterns", "Cluster incidents by severity and reproducibility.", TaskPriority.Medium, TaskStatus.Todo, qa.Id, 6),
                ("Publish weekly digest", "Summarize queue health and upcoming risks.", TaskPriority.Low, TaskStatus.Todo, productOwner.Id, 7)
            ],
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Master data seeding completed successfully.");
    }

    private async Task<ApplicationUser> EnsureUserAsync(string email, string firstName, string lastName)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            return existingUser;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, _settings.DefaultPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Failed to seed user '{email}': {errors}");
        }

        return user;
    }

    private async Task SeedProjectAsync(
        string name,
        string description,
        string ownerId,
        IReadOnlyList<(string Title, string Description, TaskPriority Priority, TaskStatus Status, string AssigneeId, int DueInDays)> tasks,
        CancellationToken cancellationToken)
    {
        var projectResult = Project.Create(name, description, ownerId);
        if (projectResult.IsFailure)
        {
            throw new InvalidOperationException(projectResult.Error?.Description ?? "Failed to create seed project.");
        }

        var project = projectResult.Value!;
        await projectRepository.AddAsync(project, cancellationToken);

        foreach (var item in tasks)
        {
            var taskResult = ProjectTask.Create(
                item.Title,
                item.Description,
                item.Priority,
                DateTime.UtcNow.Date.AddDays(item.DueInDays),
                item.AssigneeId,
                project.Id);

            if (taskResult.IsFailure)
            {
                throw new InvalidOperationException(taskResult.Error?.Description ?? "Failed to create seed task.");
            }

            var task = taskResult.Value!;
            if (task.Status != item.Status)
            {
                task.ChangeStatus(item.Status);
            }

            await taskRepository.AddAsync(task, cancellationToken);
        }
    }
}
