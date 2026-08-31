namespace SlientMoon.Application.DTOs.Account
{
    public class AdminAuthenticationResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; } = 900;
    }
}
