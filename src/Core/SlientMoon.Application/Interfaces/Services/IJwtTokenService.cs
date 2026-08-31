using SlientMoon.Domain.Entities;
using System.Security.Claims;

namespace SlientMoon.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        public string GenerateAccessToken(ApplicationUser user);
        public string GenerateRefreshToken();
        public string GetUserIdFromToken(string token);
        public ClaimsPrincipal ValidateToken(string token);
    }
}
