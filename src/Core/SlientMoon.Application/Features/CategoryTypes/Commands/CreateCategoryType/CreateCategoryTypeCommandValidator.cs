using FluentValidation;

namespace SlientMoon.Application.Features.CategoryTypes.Commands.CreateCategoryType
{
    public class CreateCategoryTypeCommandValidator : AbstractValidator<CreateCategoryTypeCommand>
    {
        public CreateCategoryTypeCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ad daxil edin").MaximumLength(100);
            RuleFor(x => x.Slug).NotEmpty().WithMessage("Slug daxil edin").MaximumLength(100);
        }
    }
}