using SlientMoon.Domain.Enums;
using System.Collections.Generic;

namespace SlientMoon.Application.Common.Extensions
{
    public static class TranslationMapExtensions
    {
        public static string Get(
           this Dictionary<LanguageCode, Dictionary<TranslatableField, string>> map,
           LanguageCode language,
           TranslatableField field)
        {
            return map != null
                && map.TryGetValue(language, out var fields)
                && fields.TryGetValue(field, out var value)
                ? value
                : null;
        }

        public static void Set(
           this Dictionary<LanguageCode, Dictionary<TranslatableField, string>> map,
           LanguageCode language,
           TranslatableField field,
           string value)
        {
            if (!map.TryGetValue(language, out var fields))
            {
                fields = new Dictionary<TranslatableField, string>();
                map[language] = fields;
            }

            fields[field] = value;
        }

    }
}
