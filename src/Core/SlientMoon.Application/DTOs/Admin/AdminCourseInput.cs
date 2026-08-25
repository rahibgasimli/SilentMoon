namespace SlientMoon.Application.DTOs.Admin
{
    public class AdminCourseInput
    {
        public string ImageUrl { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryId { get; set; }

        public AdminCourseTranslationInput En { get; set; }
        public AdminCourseTranslationInput Az { get; set; }
        public AdminCourseTranslationInput Ru { get; set; }
    }
}
