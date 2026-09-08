using SlientMoon.Domain.Enums;
using System;
using System.Collections.Generic;

namespace SlientMoon.Application.DTOs.Home
{
    /// <summary>
    /// Home feed üçün DB-dən çəkilən sətir. Track-lərin özü yox, yalnız sayı gəlir.
    /// </summary>
    public class HomeCourseRow
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TrackCount { get; set; }
        public List<Gender> NarratorGenders { get; set; } = new();
    }
}
