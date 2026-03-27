using FluentValidation;
using TaskPro.Application.DTOs.Tasks;

namespace TaskPro.Application.Validators
{
    public class CreateTaskItemValidator : AbstractValidator<CreateTaskItemDTO>
    {
        public CreateTaskItemValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(200);

            RuleFor(x => x.ProjectId)
                .NotEmpty().WithMessage("ProjectId is required.");

            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.DueDate.HasValue)
                .WithMessage("Due date must be in the future.");
        }
    }
}
