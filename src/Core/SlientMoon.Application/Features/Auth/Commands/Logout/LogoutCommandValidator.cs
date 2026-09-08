using FluentValidation;

namespace SlientMoon.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotNull()
                .NotEmpty();
        }
    }
}
