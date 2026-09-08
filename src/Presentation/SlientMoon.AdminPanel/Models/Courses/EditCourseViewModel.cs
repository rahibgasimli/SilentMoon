using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Domain.Enums;

namespace SlientMoon.AdminPanel.Models.Courses
{
    public class EditCourseViewModel
    {
        public string Id { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryId { get; set; }
        public List<string> NarratorIds { get; set; } = new();

        public Dictionary<LanguageCode, Dictionary<TranslatableField, string>> Languages { get; set; } = new();

        public IFormFile? ImageFile { get; set; }
        public string ExistingImageUrl { get; set; }

        public List<CategoryOptionDto> CategoryOptions { get; set; } = new();
        public List<NarratorOptionDto> NarratorOptions { get; set; } = new();
    }
}
