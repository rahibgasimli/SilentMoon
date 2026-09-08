using SlientMoon.Domain.Enums;
using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Categories
{
    public class CategoryInput
    {
        public string Slug { get; set; }
        public string IconUrl { get; set; }
        public string CategoryTypeId { get; set; }

        public Dictionary<LanguageCode, Dictionary<TranslatableField, string>> Languages { get; set; } = new();
    }
}
