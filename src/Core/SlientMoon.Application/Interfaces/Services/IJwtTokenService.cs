using SlientMoon.Domain.Entities;
using System.Security.Claims;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        public string GenerateAccessToken(ApplicationUser user);
        public string GenerateAdminAccessToken(Admin admin);
        public string GenerateRefreshToken();
        public ClaimsPrincipal ValidateToken(string token);
    }
}
