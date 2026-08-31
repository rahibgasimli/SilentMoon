using System;

namespace SlientMoon.Application.DTOs.Admin
{
    public class AdminCourseListItemDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public int ViewCount { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
