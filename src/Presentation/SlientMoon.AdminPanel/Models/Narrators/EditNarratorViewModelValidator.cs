using FluentValidation;

namespace SlientMoon.AdminPanel.Models.Narrators
{
    public class EditNarratorViewModelValidator : AbstractValidator<EditNarratorViewModel>
    {
        public EditNarratorViewModelValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ad daxil edilməlidir.").MaximumLength(150);
        }
    }
}
