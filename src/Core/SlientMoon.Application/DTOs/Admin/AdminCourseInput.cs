using SlientMoon.Domain.Enums;
using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Admin
{
    public class AdminCourseInput
    {
        public string ImageUrl { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryId { get; set; }
        public Dictionary<LanguageCode, AdminCourseTranslationInput> Languages { get; set; }
    }
}
