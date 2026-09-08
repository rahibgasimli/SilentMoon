using FluentValidation;

namespace SlientMoon.AdminPanel.Models.CategoryTypes
{
    public class CreateCategoryTypeViewModelValidator : AbstractValidator<CreateCategoryTypeViewModel>
    {
        public CreateCategoryTypeViewModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ad daxil edilməlidir.").MaximumLength(100);
            RuleFor(x => x.Slug).NotEmpty().WithMessage("Slug daxil edilməlidir.").MaximumLength(100);
        }
    }
}