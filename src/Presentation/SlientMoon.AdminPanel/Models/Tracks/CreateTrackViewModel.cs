using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Domain.Enums;

namespace SlientMoon.AdminPanel.Models.Tracks
{
    public class CreateTrackViewModel
    {
        public int DurationSec { get; set; }
        public string CourseId { get; set; }
        public string NarratorId { get; set; }

        public Dictionary<LanguageCode, Dictionary<TranslatableField, string>> Languages { get; set; } = new();

        public IFormFile AudioFile { get; set; }

        public List<CourseOptionDto> CourseOptions { get; set; } = new();
        public List<NarratorOptionDto> NarratorOptions { get; set; } = new();
    }
}
