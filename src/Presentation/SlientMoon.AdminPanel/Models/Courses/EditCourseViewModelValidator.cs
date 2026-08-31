using FluentValidation;

namespace SlientMoon.AdminPanel.Models.Courses
{
    public class EditCourseViewModelValidator : AbstractValidator<EditCourseViewModel>
    {
        public EditCourseViewModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.TitleEn).NotEmpty().WithMessage("Başlıq (EN) mütləqdir.");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Kateqoriya seçilməlidir.");
            RuleFor(x => x.DurationSec).GreaterThan(0).WithMessage("Müddət 0-dan böyük olmalıdır.");
        }
    }
}