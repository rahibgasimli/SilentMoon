using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.AdminPanel.Models.Categories;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.Features.Categories.Commands.CreateCategory;
using SlientMoon.Application.Features.Categories.Commands.DeleteCategory;
using SlientMoon.Application.Features.Categories.Commands.UpdateCategory;
using SlientMoon.Application.Features.Categories.Queries.GetAdminCategories;
using SlientMoon.Application.Features.Categories.Queries.GetCategoryById;
using SlientMoon.Application.Features.CategoryTypes.Queries.GetCategoryTypeOptions;
using SlientMoon.Application.Features.Storage.Commands.UploadFile;
using SlientMoon.Domain.Enums;

namespace SlientMoon.AdminPanel.Controllers
{
    public class CategoriesController : BaseAdminController
    {
        private readonly IValidator<CreateCategoryViewModel> _createValidator;
        private readonly IValidator<EditCategoryViewModel> _editValidator;

        public CategoriesController(
            IValidator<CreateCategoryViewModel> createValidator,
            IValidator<EditCategoryViewModel> editValidator)
        {
            _createValidator = createValidator;
            _editValidator = editValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? search = null, string? categoryTypeId = null, CancellationToken ct = default)
        {
            var result = await Dispatcher.Send(new GetAdminCategoriesQuery { Page = page, Limit = 20, Search = search, CategoryTypeId = categoryTypeId }, ct);

            if (!result.IsSuccess)
                return View(new PagedResult<CategoryListItemDto>());

            ViewBag.Search = search;
            ViewBag.CategoryTypeId = categoryTypeId;
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var model = new CreateCategoryViewModel
            {
                CategoryTypeOptions = await GetCategoryTypeOptionsAsync(ct)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model, CancellationToken ct)
        {
            var validationResult = await _createValidator.ValidateAsync(model, ct);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                model.CategoryTypeOptions = await GetCategoryTypeOptionsAsync(ct);
                return View(model);
            }

            await using var stream = model.IconFile.OpenReadStream();
            var uploadResult = await Dispatcher.Send(
                new UploadFileCommand(stream, model.IconFile.FileName, model.IconFile.ContentType, StorageType.Image),
                ct);

            if (!uploadResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Icon fayl yüklənərkən xəta baş verdi.");
                model.CategoryTypeOptions = await GetCategoryTypeOptionsAsync(ct);
                return View(model);
            }

            var languages = new Dictionary<LanguageCode, CategoryTranslationInput>
            {
                [LanguageCode.EN] = new CategoryTranslationInput { Name = model.NameEn }
            };

            if (!string.IsNullOrWhiteSpace(model.NameAz))
                languages[LanguageCode.AZ] = new CategoryTranslationInput { Name = model.NameAz };

            if (!string.IsNullOrWhiteSpace(model.NameRu))
                languages[LanguageCode.RU] = new CategoryTranslationInput { Name = model.NameRu };

            var command = new CreateCategoryCommand
            {
                Slug = model.Slug,
                IconUrl = uploadResult.Value.FileName,
                CategoryTypeId = model.CategoryTypeId,
                Languages = languages
            };

            var createResult = await Dispatcher.Send(command, ct);
            if (!createResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Category yaradılarkən xəta baş verdi.");
                model.CategoryTypeOptions = await GetCategoryTypeOptionsAsync(ct);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id, CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetCategoryByIdQuery { Id = id }, ct);
            if (!result.IsSuccess)
                return NotFound();

            var category = result.Value;
            category.Languages.TryGetValue(LanguageCode.EN, out var en);
            category.Languages.TryGetValue(LanguageCode.AZ, out var az);
            category.Languages.TryGetValue(LanguageCode.RU, out var ru);

            var model = new EditCategoryViewModel
            {
                Id = id,
                Slug = category.Slug,
                ExistingIconUrl = category.IconUrl,
                CategoryTypeId = category.CategoryTypeId,
                NameEn = en?.Name,
                NameAz = az?.Name,
                NameRu = ru?.Name,
                CategoryTypeOptions = await GetCategoryTypeOptionsAsync(ct)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryViewModel model, CancellationToken ct)
        {
            var validationResult = await _editValidator.ValidateAsync(model, ct);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                model.CategoryTypeOptions = await GetCategoryTypeOptionsAsync(ct);
                return View(model);
            }

            var iconUrl = model.ExistingIconUrl;
            var iconReplaced = false;

            if (model.IconFile is not null)
            {
                await using var stream = model.IconFile.OpenReadStream();
                var uploadResult = await Dispatcher.Send(
                    new UploadFileCommand(stream, model.IconFile.FileName, model.IconFile.ContentType, StorageType.Image),
                    ct);

                if (!uploadResult.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, "Icon fayl yüklənərkən xəta baş verdi.");
                    model.CategoryTypeOptions = await GetCategoryTypeOptionsAsync(ct);
                    return View(model);
                }

                iconUrl = uploadResult.Value.FileName;
                iconReplaced = true;
            }

            var languages = new Dictionary<LanguageCode, CategoryTranslationInput>
            {
                [LanguageCode.EN] = new CategoryTranslationInput { Name = model.NameEn }
            };

            if (!string.IsNullOrWhiteSpace(model.NameAz))
                languages[LanguageCode.AZ] = new CategoryTranslationInput { Name = model.NameAz };

            if (!string.IsNullOrWhiteSpace(model.NameRu))
                languages[LanguageCode.RU] = new CategoryTranslationInput { Name = model.NameRu };

            var command = new UpdateCategoryCommand
            {
                Id = model.Id,
                Slug = model.Slug,
                IconUrl = iconUrl,
                CategoryTypeId = model.CategoryTypeId,
                IconReplaced = iconReplaced,
                Languages = languages
            };

            var updateResult = await Dispatcher.Send(command, ct);
            if (!updateResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Category yenilənərkən xəta baş verdi.");
                model.CategoryTypeOptions = await GetCategoryTypeOptionsAsync(ct);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var deleteResult = await Dispatcher.Send(new DeleteCategoryCommand { Id = id }, ct);
            if (!deleteResult.IsSuccess)
                TempData["Error"] = "Category silinərkən xəta baş verdi (ona bağlı course-lar ola bilər).";

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<CategoryTypeOptionDto>> GetCategoryTypeOptionsAsync(CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetCategoryTypeOptionsQuery(), ct);
            return result.IsSuccess ? result.Value : new List<CategoryTypeOptionDto>();
        }
    }
}
