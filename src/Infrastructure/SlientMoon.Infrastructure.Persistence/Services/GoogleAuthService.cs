using Google.Apis.Auth;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Errors;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private const string GoogleClientId = "184413544217-bgo5vnnsna876mm9umi6k35akjf9sg5c.apps.googleusercontent.com";

        public async Task<Result<GoogleUserPayload>> VerifyTokenAsync(string idToken)
        {
            if (string.IsNullOrWhiteSpace(idToken))
                return UserErrors.InvalidGoogleToken;

            GoogleJsonWebSignature.Payload payload;

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { GoogleClientId }
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (InvalidJwtException)
            {
                return UserErrors.InvalidGoogleToken;
            }

            var googleUser = new GoogleUserPayload
            {
                Email = payload.Email,
                Name = payload.Name,
                AvatarUrl = payload.Picture
            };

            return Result.Success(googleUser);
        }
    }
}
