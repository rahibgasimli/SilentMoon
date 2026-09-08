using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.AdminPanel.Models.Narrators;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.DTOs.Narrators;
using SlientMoon.Application.Features.Narrators.Commands.CreateNarrator;
using SlientMoon.Application.Features.Narrators.Commands.DeleteNarrator;
using SlientMoon.Application.Features.Narrators.Commands.UpdateNarrator;
using SlientMoon.Application.Features.Narrators.Queries.GetNarratorById;
using SlientMoon.Application.Features.Narrators.Queries.GetNarrators;

namespace SlientMoon.AdminPanel.Controllers
{
    public class NarratorsController : BaseAdminController
    {
        private readonly IValidator<CreateNarratorViewModel> _createValidator;
        private readonly IValidator<EditNarratorViewModel> _editValidator;

        public NarratorsController(
            IValidator<CreateNarratorViewModel> createValidator,
            IValidator<EditNarratorViewModel> editValidator)
        {
            _createValidator = createValidator;
            _editValidator = editValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? search = null, CancellationToken ct = default)
        {
            var result = await Dispatcher.Send(new GetNarratorsQuery { Page = page, Limit = 20, Search = search }, ct);

            if (!result.IsSuccess)
                return View(new PagedResult<NarratorListItemDto>());

            ViewBag.Search = search;
            return View(result.Value);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateNarratorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNarratorViewModel model, CancellationToken ct)
        {
            var validationResult = await _createValidator.ValidateAsync(model, ct);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                return View(model);
            }

            var command = new CreateNarratorCommand { Name = model.Name, Gender = model.Gender };
            var createResult = await Dispatcher.Send(command, ct);
            if (!createResult.IsSuccess)
            {
                AddResultErrorsToModelState(createResult.Error);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id, CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetNarratorByIdQuery { Id = id }, ct);
            if (!result.IsSuccess)
                return NotFound();

            var model = new EditNarratorViewModel
            {
                Id = id,
                Name = result.Value.Name,
                Gender = result.Value.Gender
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditNarratorViewModel model, CancellationToken ct)
        {
            var validationResult = await _editValidator.ValidateAsync(model, ct);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                return View(model);
            }

            var command = new UpdateNarratorCommand { Id = model.Id, Name = model.Name, Gender = model.Gender };
            var updateResult = await Dispatcher.Send(command, ct);
            if (!updateResult.IsSuccess)
            {
                AddResultErrorsToModelState(updateResult.Error);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var deleteResult = await Dispatcher.Send(new DeleteNarratorCommand { Id = id }, ct);
            if (!deleteResult.IsSuccess)
                TempData["Error"] = deleteResult.Error.Description;

            return RedirectToAction(nameof(Index));
        }
    }
}
