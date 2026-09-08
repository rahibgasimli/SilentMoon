using FluentValidation;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Domain.Enums;

namespace SlientMoon.Application.Features.Admin.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            RuleFor(x => x.DurationSec)
                .GreaterThan(0).WithMessage("Müddət 0-dan böyük olmalıdır");

            RuleFor(x => x.Languages)
                .Must(languages => languages != null && languages.ContainsKey(LanguageCode.EN))
                .WithMessage("İngilis dili məlumatları məcburidir");

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.EN), () =>
            {
                RuleFor(x => x.Languages.Get(LanguageCode.EN, TranslatableField.Title))
                    .NotEmpty().WithMessage("Başlıq (EN) daxil edin")
                    .MaximumLength(200)
                    .WithName("Title_EN");

                RuleFor(x => x.Languages.Get(LanguageCode.EN, TranslatableField.Subtitle))
                    .MaximumLength(2000)
                    .WithName("Subtitle_EN");

                RuleFor(x => x.Languages.Get(LanguageCode.EN, TranslatableField.Description))
                    .MaximumLength(2000)
                    .WithName("Description_EN");
            });

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.AZ), () =>
            {
                RuleFor(x => x.Languages.Get(LanguageCode.AZ, TranslatableField.Title))
                    .NotEmpty().WithMessage("Başlıq (AZ) daxil edin, ya da AZ tabını tam boş buraxın")
                    .MaximumLength(250)
                    .WithName("Title_AZ");
            });

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.RU), () =>
            {
                RuleFor(x => x.Languages.Get(LanguageCode.RU, TranslatableField.Title))
                    .NotEmpty().WithMessage("Başlıq (RU) daxil edin, ya da RU tabını tam boş buraxın")
                    .MaximumLength(250)
                    .WithName("Title_RU");
            });
        }
    }
}