using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.AdminPanel.Models.Courses;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.Features.Admin.Courses.Commands.CreateCourse;
using SlientMoon.Application.Features.Admin.Courses.Commands.DeleteCourse;
using SlientMoon.Application.Features.Admin.Courses.Commands.UpdateCourse;
using SlientMoon.Application.Features.Admin.Courses.Queries.GetAdminCourses;
using SlientMoon.Application.Features.Admin.Courses.Queries.GetCategoryOptions;
using SlientMoon.Application.Features.Admin.Courses.Queries.GetCourseById;
using SlientMoon.Application.Features.Admin.Tracks.Queries.GetNarratorOptions;
using SlientMoon.Application.Features.Storage.Commands.UploadFile;
using SlientMoon.Domain.Enums;

namespace SlientMoon.AdminPanel.Controllers
{
    public class CoursesController : BaseAdminController
    {
        private readonly IValidator<CreateCourseViewModel> _createValidator;

        public CoursesController(IValidator<CreateCourseViewModel> createValidator)
        {
            _createValidator = createValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? search = null, CancellationToken ct = default)
        {
            var result = await Dispatcher.Send(new GetAdminCoursesQuery { Page = page, Limit = 20, Search = search }, ct);

            if (!result.IsSuccess)
                return View(new PagedResult<AdminCourseListItemDto>());

            ViewBag.Search = search;
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var model = new CreateCourseViewModel
            {
                CategoryOptions = await GetCategoryOptionsAsync(ct),
                NarratorOptions = await GetNarratorOptionsAsync(ct)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseViewModel model, CancellationToken ct)
        {
            var validationResult = await _createValidator.ValidateAsync(model, ct);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                model.CategoryOptions = await GetCategoryOptionsAsync(ct);
                model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                return View(model);
            }

            await using var stream = model.ImageFile.OpenReadStream();
            var uploadResult = await Dispatcher.Send(
                new UploadFileCommand(stream, model.ImageFile.FileName, model.ImageFile.ContentType, StorageType.Image),
                ct);

            if (!uploadResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Şəkil yüklənərkən xəta baş verdi.");
                model.CategoryOptions = await GetCategoryOptionsAsync(ct);
                model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                return View(model);
            }

            var command = new CreateCourseCommand
            {
                ImageUrl = uploadResult.Value.FileName,
                DurationSec = model.DurationSec,
                IsFeatured = model.IsFeatured,
                CategoryId = model.CategoryId,
                NarratorIds = model.NarratorIds,
                Languages = model.Languages
            };

            var createResult = await Dispatcher.Send(command, ct);
            if (!createResult.IsSuccess)
            {
                AddResultErrorsToModelState(createResult.Error);
                model.CategoryOptions = await GetCategoryOptionsAsync(ct);
                model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id, CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetCourseByIdQuery { Id = id }, ct);
            if (!result.IsSuccess)
                return NotFound();

            var course = result.Value;

            var model = new EditCourseViewModel
            {
                Id = id,
                ExistingImageUrl = course.ImageUrl,
                DurationSec = course.DurationSec,
                IsFeatured = course.IsFeatured,
                CategoryId = course.CategoryId,
                NarratorIds = course.NarratorIds,
                Languages = course.Languages,
                CategoryOptions = await GetCategoryOptionsAsync(ct),
                NarratorOptions = await GetNarratorOptionsAsync(ct)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCourseViewModel model, CancellationToken ct)
        {
            var imageUrl = model.ExistingImageUrl;

            if (model.ImageFile is not null)
            {
                await using var stream = model.ImageFile.OpenReadStream();
                var uploadResult = await Dispatcher.Send(
                    new UploadFileCommand(stream, model.ImageFile.FileName, model.ImageFile.ContentType, StorageType.Image),
                    ct);

                if (!uploadResult.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, "Şəkil yüklənərkən xəta baş verdi.");
                    model.CategoryOptions = await GetCategoryOptionsAsync(ct);
                    model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                    return View(model);
                }

                imageUrl = uploadResult.Value.FileName;
            }

            var command = new UpdateCourseCommand
            {
                Id = model.Id,
                ImageUrl = imageUrl,
                DurationSec = model.DurationSec,
                IsFeatured = model.IsFeatured,
                CategoryId = model.CategoryId,
                NarratorIds = model.NarratorIds,
                Languages = model.Languages
            };

            var updateResult = await Dispatcher.Send(command, ct);
            if (!updateResult.IsSuccess)
            {
                AddResultErrorsToModelState(updateResult.Error);
                model.CategoryOptions = await GetCategoryOptionsAsync(ct);
                model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var deleteResult = await Dispatcher.Send(new DeleteCourseCommand { Id = id }, ct);
            if (!deleteResult.IsSuccess)
                TempData["Error"] = "Kurs silinərkən xəta baş verdi.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<CategoryOptionDto>> GetCategoryOptionsAsync(CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetCategoryOptionsQuery(), ct);
            return result.IsSuccess ? result.Value : new List<CategoryOptionDto>();
        }

        private async Task<List<NarratorOptionDto>> GetNarratorOptionsAsync(CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetNarratorOptionsQuery(), ct);
            return result.IsSuccess ? result.Value : new List<NarratorOptionDto>();
        }
    }
}
