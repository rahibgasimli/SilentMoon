using FluentValidation;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Domain.Enums;

namespace SlientMoon.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.CategoryTypeId).NotEmpty().WithMessage("Category type seçilməlidir");
            RuleFor(x => x.Slug).NotEmpty().WithMessage("Slug daxil edin").MaximumLength(150);

            RuleFor(x => x.Languages)
                .Must(languages => languages != null && languages.ContainsKey(LanguageCode.EN))
                .WithMessage("İngilis dili məlumatları məcburidir");

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.EN), () =>
            {
                RuleFor(x => x.Languages.Get(LanguageCode.EN, TranslatableField.Name))
                    .NotEmpty().WithMessage("Ad (EN) daxil edin")
                    .MaximumLength(150)
                    .WithName("Name_EN");
            });

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.AZ), () =>
            {
                RuleFor(x => x.Languages.Get(LanguageCode.AZ, TranslatableField.Name))
                    .NotEmpty().WithMessage("Ad (AZ) daxil edin, ya da AZ tabını tam boş buraxın")
                    .MaximumLength(150)
                    .WithName("Name_AZ");
            });

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.RU), () =>
            {
                RuleFor(x => x.Languages.Get(LanguageCode.RU, TranslatableField.Name))
                    .NotEmpty().WithMessage("Ad (RU) daxil edin, ya da RU tabını tam boş buraxın")
                    .MaximumLength(150)
                    .WithName("Name_RU");
            });
        }
    }
}
