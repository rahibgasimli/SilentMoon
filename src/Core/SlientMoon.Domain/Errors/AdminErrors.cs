namespace SlientMoon.Domain.Errors
{
    public static class AdminErrors
    {
        public static readonly Error InvalidCredentials = Error.Validation(
            "Admins.InvalidCredentials",
            "Invalid email or password");
    }
}
