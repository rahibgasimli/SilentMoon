using FluentValidation;

namespace SlientMoon.AdminPanel.Models.Categories
{
    public class CreateCategoryViewModelValidator : AbstractValidator<CreateCategoryViewModel>
    {
        public CreateCategoryViewModelValidator()
        {
            RuleFor(x => x.NameEn).NotEmpty().WithMessage("Ad (EN) mütləqdir.");
            RuleFor(x => x.Slug).NotEmpty().WithMessage("Slug daxil edilməlidir.");
            RuleFor(x => x.CategoryTypeId).NotEmpty().WithMessage("Category type seçilməlidir.");
            RuleFor(x => x.IconFile).NotNull().WithMessage("Icon fayl seçilməlidir.");
        }
    }
}
