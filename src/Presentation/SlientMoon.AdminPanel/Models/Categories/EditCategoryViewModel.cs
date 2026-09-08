using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Domain.Enums;

namespace SlientMoon.AdminPanel.Models.Categories
{
    public class EditCategoryViewModel
    {
        public string Id { get; set; }

        public string Slug { get; set; }
        public string CategoryTypeId { get; set; }

        public Dictionary<LanguageCode, Dictionary<TranslatableField, string>> Languages { get; set; } = new();

        public string ExistingIconUrl { get; set; }
        public IFormFile? IconFile { get; set; }

        public List<CategoryTypeOptionDto> CategoryTypeOptions { get; set; } = new();
    }
}
