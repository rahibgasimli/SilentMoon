using FluentValidation;

namespace SlientMoon.AdminPanel.Models.Courses
{
    public class CreateCourseViewModelValidator : AbstractValidator<CreateCourseViewModel>
    {
        public CreateCourseViewModelValidator()
        {
            RuleFor(x => x.ImageFile).NotNull().WithMessage("Şəkil seçilməlidir.");
        }
    }
}
