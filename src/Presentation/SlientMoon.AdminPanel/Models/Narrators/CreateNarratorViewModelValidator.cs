using FluentValidation;

namespace SlientMoon.AdminPanel.Models.Narrators
{
    public class CreateNarratorViewModelValidator : AbstractValidator<CreateNarratorViewModel>
    {
        public CreateNarratorViewModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ad daxil edilməlidir.").MaximumLength(150);
        }
    }
}
