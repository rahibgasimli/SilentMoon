using FluentValidation;

namespace SlientMoon.AdminPanel.Models.Tracks
{
    public class CreateTrackViewModelValidator : AbstractValidator<CreateTrackViewModel>
    {
        public CreateTrackViewModelValidator()
        {
            RuleFor(x => x.AudioFile).NotNull().WithMessage("Audio fayl seçilməlidir.");
        }
    }
}
