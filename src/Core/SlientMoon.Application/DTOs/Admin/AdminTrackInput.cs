using SlientMoon.Domain.Enums;
using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Admin
{
    public class AdminTrackInput
    {
        public int DurationSec { get; set; }
        public string AudioUrl { get; set; }
        public string MimeType { get; set; }
        public long FileSizeBytes { get; set; }

        public string CourseId { get; set; }
        public string NarratorId { get; set; }

        public Dictionary<LanguageCode, Dictionary<TranslatableField, string>> Languages { get; set; } = new();
    }
}
