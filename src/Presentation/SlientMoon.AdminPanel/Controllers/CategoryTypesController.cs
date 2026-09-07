using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.AdminPanel.Models.CategoryTypes;
using SlientMoon.Application.DTOs.CategoryType;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.Features.CategoryTypes.Commands.CreateCategoryType;
using SlientMoon.Application.Features.CategoryTypes.Commands.DeleteCategoryType;
using SlientMoon.Application.Features.CategoryTypes.Commands.UpdateCategoryType;
using SlientMoon.Application.Features.CategoryTypes.Queries.GetCategoryTypeById;
using SlientMoon.Application.Features.CategoryTypes.Queries.GetCategoryTypes;

namespace SlientMoon.AdminPanel.Controllers
{
    public class CategoryTypesController : BaseAdminController
    {
        private readonly IValidator<CreateCategoryTypeViewModel> _createValidator;
        private readonly IValidator<EditCategoryTypeViewModel> _editValidator;

        public CategoryTypesController(
            IValidator<CreateCategoryTypeViewModel> createValidator,
            IValidator<EditCategoryTypeViewModel> editValidator)
        {
            _createValidator = createValidator;
            _editValidator = editValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? search = null, CancellationToken ct = default)
        {
            var result = await Dispatcher.Send(new GetCategoryTypesQuery { Page = page, Limit = 20, Search = search }, ct);

            if (!result.IsSuccess)
                return View(new PagedResult<CategoryTypeListItemDto>());

            ViewBag.Search = search;
            return View(result.Value);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCategoryTypeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryTypeViewModel model, CancellationToken ct)
        {
            var validationResult = await _createValidator.ValidateAsync(model, ct);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                return View(model);
            }

            var command = new CreateCategoryTypeCommand { Name = model.Name, Slug = model.Slug };
            var createResult = await Dispatcher.Send(command, ct);
            if (!createResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Category type yaradılarkən xəta baş verdi.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id, CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetCategoryTypeByIdQuery { Id = id }, ct);
            if (!result.IsSuccess)
                return NotFound();

            var model = new EditCategoryTypeViewModel
            {
                Id = id,
                Name = result.Value.Name,
                Slug = result.Value.Slug
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryTypeViewModel model, CancellationToken ct)
        {
            var validationResult = await _editValidator.ValidateAsync(model, ct);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                return View(model);
            }

            var command = new UpdateCategoryTypeCommand { Id = model.Id, Name = model.Name, Slug = model.Slug };
            var updateResult = await Dispatcher.Send(command, ct);
            if (!updateResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Category type yenilənərkən xəta baş verdi.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var deleteResult = await Dispatcher.Send(new DeleteCategoryTypeCommand { Id = id }, ct);
            if (!deleteResult.IsSuccess)
                TempData["Error"] = "Category type silinərkən xəta baş verdi (ona bağlı category-lər ola bilər).";

            return RedirectToAction(nameof(Index));
        }
    }
}