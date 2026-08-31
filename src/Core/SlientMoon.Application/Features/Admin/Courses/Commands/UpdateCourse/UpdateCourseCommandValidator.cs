using FluentValidation;

namespace SlientMoon.Application.Features.Admin.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Kateqoriya seçilməlidir");
            RuleFor(x => x.DurationSec).GreaterThan(0).WithMessage("Müddət 0-dan böyük olmalıdır");
            RuleFor(x => x.En).NotNull().WithMessage("İngilis dili məlumatları məcburidir");

            When(x => x.En != null, () =>
            {
                RuleFor(x => x.En.Title).NotEmpty().WithMessage("Başlıq (EN) daxil edin").MaximumLength(200);
                RuleFor(x => x.En.Subtitle).MaximumLength(2000);
                RuleFor(x => x.En.Description).MaximumLength(2000);
            });

            When(x => x.Az != null, () =>
            {
                RuleFor(x => x.Az.Title).NotEmpty().WithMessage("Başlıq (AZ) daxil edin, ya da AZ tabını tam boş buraxın").MaximumLength(250);
            });

            When(x => x.Ru != null, () =>
            {
                RuleFor(x => x.Ru.Title).NotEmpty().WithMessage("Başlıq (RU) daxil edin, ya da RU tabını tam boş buraxın").MaximumLength(250);
            });
        }
    }
}