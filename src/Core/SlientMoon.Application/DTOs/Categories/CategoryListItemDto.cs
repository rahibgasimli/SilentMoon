using System;

namespace SlientMoon.Application.DTOs.Categories
{
    public class CategoryListItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? IconUrl { get; set; }
        public string CategoryTypeId { get; set; }
        public string CategoryTypeName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
