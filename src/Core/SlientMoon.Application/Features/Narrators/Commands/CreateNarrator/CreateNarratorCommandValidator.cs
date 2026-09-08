using FluentValidation;

namespace SlientMoon.Application.Features.Narrators.Commands.CreateNarrator
{
    public class CreateNarratorCommandValidator : AbstractValidator<CreateNarratorCommand>
    {
        public CreateNarratorCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ad daxil edin").MaximumLength(150);
            RuleFor(x => x.Gender).IsInEnum().WithMessage("Cins seçilməlidir");
        }
    }
}
