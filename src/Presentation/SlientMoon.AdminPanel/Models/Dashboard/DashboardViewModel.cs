using SlientMoon.Application.DTOs.Admin;

namespace SlientMoon.AdminPanel.Models.Dashboard
{
    public class DashboardViewModel
    {
        public int CourseCount { get; set; }
        public int CategoryCount { get; set; }
        public int CategoryTypeCount { get; set; }
        public int TrackCount { get; set; }

        public List<AdminCourseListItemDto> RecentCourses { get; set; } = new();
    }
}
