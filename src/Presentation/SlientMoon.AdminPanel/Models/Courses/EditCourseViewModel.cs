using SlientMoon.Application.DTOs.Admin;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SlientMoon.AdminPanel.Models.Courses
{
    public class EditCourseViewModel
    {
        public string Id { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryId { get; set; }

        public string TitleEn { get; set; }
        public string SubtitleEn { get; set; }
        public string DescriptionEn { get; set; }

        public string TitleAz { get; set; }
        public string SubtitleAz { get; set; }
        public string DescriptionAz { get; set; }

        public string TitleRu { get; set; }
        public string SubtitleRu { get; set; }
        public string DescriptionRu { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string ExistingImageUrl { get; set; }

        public List<CategoryOptionDto> CategoryOptions { get; set; } = new();
    }
}