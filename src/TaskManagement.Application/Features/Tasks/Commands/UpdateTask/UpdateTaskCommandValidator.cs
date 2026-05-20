using FluentValidation;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required.");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(500).WithMessage("Task title cannot exceed 500 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
            .When(x => x.DueDate.HasValue);
    }
}
