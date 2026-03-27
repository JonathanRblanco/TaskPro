using FluentValidation;
using TaskPro.Application.DTOs.Projects;

namespace TaskPro.Application.Validators
{
    public class CreateProjectValidator : AbstractValidator<CreateProjectDTO>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .When(x => x.Description is not null);
        }
    }
}
