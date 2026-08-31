using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.AdminPanel.Models.Courses;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.Features.Admin.Courses.Commands.CreateCourse;
using SlientMoon.Application.Features.Admin.Courses.Queries.GetAdminCourses;
using SlientMoon.Application.Features.Admin.Courses.Queries.GetCategoryOptions;
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
