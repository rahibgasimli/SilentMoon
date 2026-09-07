using FluentValidation;

namespace SlientMoon.AdminPanel.Models.Tracks
{
    public class EditTrackViewModelValidator : AbstractValidator<EditTrackViewModel>
    {
        public EditTrackViewModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.TitleEn).NotEmpty().WithMessage("Başlıq (EN) mütləqdir.");
            RuleFor(x => x.CourseId).NotEmpty().WithMessage("Kurs seçilməlidir.");
            RuleFor(x => x.NarratorId).NotEmpty().WithMessage("Narrator seçilməlidir.");
            RuleFor(x => x.DurationSec).GreaterThan(0).WithMessage("Müddət 0-dan böyük olmalıdır.");
        }
    }
}
