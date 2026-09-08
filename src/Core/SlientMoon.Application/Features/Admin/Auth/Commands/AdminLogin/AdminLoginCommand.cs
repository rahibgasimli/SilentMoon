using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Account;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Auth.Commands.AdminLogin
{
    public class AdminLoginCommand : ICommand<AdminAuthenticationResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AdminLoginCommandHandler : ICommandHandler<AdminLoginCommand, AdminAuthenticationResponse>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUow _uow;
        private readonly IAppLogger<AdminLoginCommandHandler> _logger;

        public AdminLoginCommandHandler(
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IUow uow,
            IAppLogger<AdminLoginCommandHandler> logger)
        {
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _uow = uow;
            _logger = logger;
        }


        public async Task<Result<AdminAuthenticationResponse>> Handle(AdminLoginCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Admin login started. Email: {Email}", command.Email);

            var admin = await _uow.AdminRepository.GetByEmailAsync(command.Email);

            if (admin is null || !_passwordHasher.Verify(command.Password, admin.PasswordHash))
            {
                _logger.LogWarning("Admin login failed. Email: {Email}", command.Email);
                return AdminErrors.InvalidCredentials;
            }

            var accessToken = _jwtTokenService.GenerateAdminAccessToken(admin);

            _logger.LogInformation("Admin login successful. Email: {Email}", command.Email);

            return new AdminAuthenticationResponse
            {
                AccessToken = accessToken
            };
        }
    }
}
