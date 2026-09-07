using SlientMoon.Application.DTOs.Admin;

namespace SlientMoon.AdminPanel.Models.Tracks
{
    public class EditTrackViewModel
    {
        public string Id { get; set; }

        public int DurationSec { get; set; }
        public string CourseId { get; set; }
        public string NarratorId { get; set; }

        public string TitleEn { get; set; }
        public string TitleAz { get; set; }
        public string TitleRu { get; set; }

        public string ExistingAudioUrl { get; set; }
        public IFormFile? AudioFile { get; set; }

        public List<CourseOptionDto> CourseOptions { get; set; } = new();
        public List<NarratorOptionDto> NarratorOptions { get; set; } = new();
    }
}
