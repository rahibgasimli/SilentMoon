using SlientMoon.Application.DTOs.Categories;

namespace SlientMoon.AdminPanel.Models.Categories
{
    public class EditCategoryViewModel
    {
        public string Id { get; set; }

        public string Slug { get; set; }
        public string CategoryTypeId { get; set; }

        public string NameEn { get; set; }
        public string NameAz { get; set; }
        public string NameRu { get; set; }

        public string ExistingIconUrl { get; set; }
        public IFormFile? IconFile { get; set; }

        public List<CategoryTypeOptionDto> CategoryTypeOptions { get; set; } = new();
    }
}
