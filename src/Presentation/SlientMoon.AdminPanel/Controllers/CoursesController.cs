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
using SlientMoon.Application.Features.Storage.Commands.UploadFile;
using SlientMoon.Domain.Enums;

namespace SlientMoon.AdminPanel.Controllers
{
    public class CoursesController : BaseAdminController
    {
        private readonly IValidator<CreateCourseViewModel> _createValidator;
        private readonly IValidator<EditCourseViewModel> _editValidator;

        public CoursesController(
            IValidator<CreateCourseViewModel> createValidator,
            IValidator<EditCourseViewModel> editValidator)
        {
            _createValidator = createValidator;
            _editValidator = editValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var model = new CreateCourseViewModel
            {
                CategoryOptions = await GetCategoryOptionsAsync(ct)
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
                return View(model);
            }

            var command = new CreateCourseCommand
            {
                ImageUrl = uploadResult.Value.FileName,
                DurationSec = model.DurationSec,
                IsFeatured = model.IsFeatured,
                CategoryId = model.CategoryId,
                En = new AdminCourseTranslationInput
                {
                    Title = model.TitleEn,
                    Subtitle = model.SubtitleEn,
                    Description = model.DescriptionEn
                },
                Az = string.IsNullOrWhiteSpace(model.TitleAz) ? null : new AdminCourseTranslationInput
                {
                    Title = model.TitleAz,
                    Subtitle = model.SubtitleAz,
                    Description = model.DescriptionAz
                },
                Ru = string.IsNullOrWhiteSpace(model.TitleRu) ? null : new AdminCourseTranslationInput
                {
                    Title = model.TitleRu,
                    Subtitle = model.SubtitleRu,
                    Description = model.DescriptionRu
                }
            };

            var createResult = await Dispatcher.Send(command, ct);
            if (!createResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Kurs yaradılarkən xəta baş verdi.");
                model.CategoryOptions = await GetCategoryOptionsAsync(ct);
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
                TitleEn = course.En.Title,
                SubtitleEn = course.En.Subtitle,
                DescriptionEn = course.En.Description,
                TitleAz = course.Az.Title,
                SubtitleAz = course.Az.Subtitle,
                DescriptionAz = course.Az.Description,
                TitleRu = course.Ru.Title,
                SubtitleRu = course.Ru.Subtitle,
                DescriptionRu = course.Ru.Description,
                CategoryOptions = await GetCategoryOptionsAsync(ct)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCourseViewModel model, CancellationToken ct)
        {
            var validationResult = await _editValidator.ValidateAsync(model, ct);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                model.CategoryOptions = await GetCategoryOptionsAsync(ct);
                return View(model);
            }

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
                En = new AdminCourseTranslationInput
                {
                    Title = model.TitleEn,
                    Subtitle = model.SubtitleEn,
                    Description = model.DescriptionEn
                },
                Az = string.IsNullOrWhiteSpace(model.TitleAz) ? null : new AdminCourseTranslationInput
                {
                    Title = model.TitleAz,
                    Subtitle = model.SubtitleAz,
                    Description = model.DescriptionAz
                },
                Ru = string.IsNullOrWhiteSpace(model.TitleRu) ? null : new AdminCourseTranslationInput
                {
                    Title = model.TitleRu,
                    Subtitle = model.SubtitleRu,
                    Description = model.DescriptionRu
                }
            };

            var updateResult = await Dispatcher.Send(command, ct);
            if (!updateResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Kurs yenilənərkən xəta baş verdi.");
                model.CategoryOptions = await GetCategoryOptionsAsync(ct);
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

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? search = null, CancellationToken ct = default)
        {
            var result = await Dispatcher.Send(new GetAdminCoursesQuery { Page = page, Limit = 20, Search = search }, ct);

            if (!result.IsSuccess)
                return View(new PagedResult<AdminCourseListItemDto>());

            ViewBag.Search = search;
            return View(result.Value);
        }

        private async Task<List<CategoryOptionDto>> GetCategoryOptionsAsync(CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetCategoryOptionsQuery(), ct);
            return result.IsSuccess ? result.Value : new List<CategoryOptionDto>();
        }
    }
}
