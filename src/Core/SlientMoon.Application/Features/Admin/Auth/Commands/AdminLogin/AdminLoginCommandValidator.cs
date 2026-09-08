using FluentValidation;

namespace SlientMoon.Application.Features.Admin.Auth.Commands.AdminLogin
{
    public class AdminLoginCommandValidator : AbstractValidator<AdminLoginCommand>
    {
        public AdminLoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
