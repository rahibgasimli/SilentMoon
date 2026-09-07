using FluentValidation;
using SlientMoon.Domain.Enums;

namespace SlientMoon.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryTypeId).NotEmpty().WithMessage("Category type seçilməlidir");
            RuleFor(x => x.Slug).NotEmpty().WithMessage("Slug daxil edin").MaximumLength(150);

            RuleFor(x => x.Languages)
                .Must(languages => languages != null && languages.ContainsKey(LanguageCode.EN))
                .WithMessage("İngilis dili məlumatları məcburidir");

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.EN), () =>
            {
                RuleFor(x => x.Languages[LanguageCode.EN].Name)
                    .NotEmpty().WithMessage("Ad (EN) daxil edin")
                    .MaximumLength(150);
            });

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.AZ), () =>
            {
                RuleFor(x => x.Languages[LanguageCode.AZ].Name)
                    .NotEmpty().WithMessage("Ad (AZ) daxil edin, ya da AZ tabını tam boş buraxın")
                    .MaximumLength(150);
            });

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.RU), () =>
            {
                RuleFor(x => x.Languages[LanguageCode.RU].Name)
                    .NotEmpty().WithMessage("Ad (RU) daxil edin, ya da RU tabını tam boş buraxın")
                    .MaximumLength(150);
            });
        }
    }
}
