using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Enums;
using System.Globalization;

namespace SlientMoon.Infrastructure.Persistence.Services
{
    public class LanguageService : ILanguageService
    {
        public LanguageCode ValidateLanguage(string? lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
                return LanguageCode.EN;

            var cleanLang = lang.Split(',')[0].Split('-')[0].Trim().ToLower();

            return cleanLang switch
            {
                "az" => LanguageCode.AZ,
                "ru" => LanguageCode.RU,
                _ => LanguageCode.EN
            };
        }

        public LanguageCode GetCurrentLanguage()
        {
            return ValidateLanguage(CultureInfo.CurrentCulture.Name);
        }
    }
}
