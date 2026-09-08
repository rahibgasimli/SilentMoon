using FluentValidation;

namespace SlientMoon.Application.Features.Narrators.Commands.UpdateNarrator
{
    public class UpdateNarratorCommandValidator : AbstractValidator<UpdateNarratorCommand>
    {
        public UpdateNarratorCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ad daxil edin").MaximumLength(150);
            RuleFor(x => x.Gender).IsInEnum().WithMessage("Cins seçilməlidir");
        }
    }
}
