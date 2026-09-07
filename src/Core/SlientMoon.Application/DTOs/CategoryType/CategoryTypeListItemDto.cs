using System;

namespace SlientMoon.Application.DTOs.CategoryType
{
    public class CategoryTypeListItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int CategoryCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
