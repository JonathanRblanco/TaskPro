using FluentValidation;
using TaskPro.Application.DTOs.Comments;

namespace TaskPro.Application.Validators
{
    public class CreateCommentValidator : AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required.")
                .MaximumLength(2000);
        }
    }
}
