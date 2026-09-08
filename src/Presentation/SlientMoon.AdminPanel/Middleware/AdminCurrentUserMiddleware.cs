using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Infrastructure.Persistence.Services;
using System.IdentityModel.Tokens.Jwt;

namespace SlientMoon.AdminPanel.Middleware
{
    public class AdminCurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminCurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
        {
            if (context.User.Identity?.IsAuthenticated == true && currentUserService is CurrentUserService service)
            {
                var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    service.SetUser(userId);
                }
            }

            await _next(context);
        }
    }
}
