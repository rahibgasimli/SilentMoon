using System;

namespace SlientMoon.Application.DTOs.Admin
{
    public class AdminTrackListItemDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int DurationSec { get; set; }
        public string AudioUrl { get; set; }
        public string MimeType { get; set; }
        public long FileSizeBytes { get; set; }

        public string CourseId { get; set; }
        public string CourseTitle { get; set; }

        public string NarratorId { get; set; }
        public string NarratorName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
