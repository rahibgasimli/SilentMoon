using FluentValidation;

namespace SlientMoon.AdminPanel.Models.Categories
{
    public class CreateCategoryViewModelValidator : AbstractValidator<CreateCategoryViewModel>
    {
        public CreateCategoryViewModelValidator()
        {
            RuleFor(x => x.IconFile).NotNull().WithMessage("Icon fayl seçilməlidir.");
        }
    }
}
