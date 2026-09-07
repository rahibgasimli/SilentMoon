using FluentValidation;

namespace SlientMoon.AdminPanel.Models.Categories
{
    public class EditCategoryViewModelValidator : AbstractValidator<EditCategoryViewModel>
    {
        public EditCategoryViewModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.NameEn).NotEmpty().WithMessage("Ad (EN) mütləqdir.");
            RuleFor(x => x.Slug).NotEmpty().WithMessage("Slug daxil edilməlidir.");
            RuleFor(x => x.CategoryTypeId).NotEmpty().WithMessage("Category type seçilməlidir.");
        }
    }
}
