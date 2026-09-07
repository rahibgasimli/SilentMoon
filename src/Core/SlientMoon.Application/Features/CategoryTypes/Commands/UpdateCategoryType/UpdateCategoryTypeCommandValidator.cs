using FluentValidation;

namespace SlientMoon.Application.Features.CategoryTypes.Commands.UpdateCategoryType
{
    public class UpdateCategoryTypeCommandValidator : AbstractValidator<UpdateCategoryTypeCommand>
    {
        public UpdateCategoryTypeCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ad daxil edin").MaximumLength(100);
            RuleFor(x => x.Slug).NotEmpty().WithMessage("Slug daxil edin").MaximumLength(100);
        }
    }
}