using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Domain.Enums;

namespace SlientMoon.AdminPanel.Models.Categories
{
    public class CreateCategoryViewModel
    {
        public string Slug { get; set; }
        public string CategoryTypeId { get; set; }

        public Dictionary<LanguageCode, Dictionary<TranslatableField, string>> Languages { get; set; } = new();

        public IFormFile IconFile { get; set; }

        public List<CategoryTypeOptionDto> CategoryTypeOptions { get; set; } = new();
    }
}
