using FluentValidation;
using SlientMoon.Domain.Enums;

namespace SlientMoon.Application.Features.Admin.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Kateqoriya seçilməlidir");

            RuleFor(x => x.DurationSec)
                .GreaterThan(0).WithMessage("Müddət 0-dan böyük olmalıdır");

            RuleFor(x => x.Languages)
                .Must(languages => languages != null && languages.ContainsKey(LanguageCode.EN))
                .WithMessage("İngilis dili məlumatları məcburidir");

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.EN), () =>
            {
                RuleFor(x => x.Languages[LanguageCode.EN].Title)
                    .NotEmpty().WithMessage("Başlıq (EN) daxil edin")
                    .MaximumLength(200);

                RuleFor(x => x.Languages[LanguageCode.EN].Subtitle)
                    .MaximumLength(2000);

                RuleFor(x => x.Languages[LanguageCode.EN].Description)
                    .MaximumLength(2000);
            });

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.AZ), () =>
            {
                RuleFor(x => x.Languages[LanguageCode.AZ].Title)
                    .NotEmpty().WithMessage("Başlıq (AZ) daxil edin, ya da AZ tabını tam boş buraxın")
                    .MaximumLength(250);
            });

            When(x => x.Languages != null && x.Languages.ContainsKey(LanguageCode.RU), () =>
            {
                RuleFor(x => x.Languages[LanguageCode.RU].Title)
                    .NotEmpty().WithMessage("Başlıq (RU) daxil edin, ya da RU tabını tam boş buraxın")
                    .MaximumLength(250);
            });
        }
    }
}