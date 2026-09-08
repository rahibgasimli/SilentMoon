using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.AdminPanel.Models.Tracks;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.Features.Admin.Tracks.Commands.CreateTrack;
using SlientMoon.Application.Features.Admin.Tracks.Commands.DeleteTrack;
using SlientMoon.Application.Features.Admin.Tracks.Commands.UpdateTrack;
using SlientMoon.Application.Features.Admin.Tracks.Queries.GetAdminTracks;
using SlientMoon.Application.Features.Admin.Tracks.Queries.GetCourseOptions;
using SlientMoon.Application.Features.Admin.Tracks.Queries.GetNarratorOptions;
using SlientMoon.Application.Features.Admin.Tracks.Queries.GetTrackById;
using SlientMoon.Application.Features.Storage.Commands.UploadFile;
using SlientMoon.Domain.Enums;

namespace SlientMoon.AdminPanel.Controllers
{
    public class TracksController : BaseAdminController
    {
        private readonly IValidator<CreateTrackViewModel> _createValidator;

        public TracksController(IValidator<CreateTrackViewModel> createValidator)
        {
            _createValidator = createValidator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? search = null, string? courseId = null, CancellationToken ct = default)
        {
            var result = await Dispatcher.Send(new GetAdminTracksQuery { Page = page, Limit = 20, Search = search, CourseId = courseId }, ct);

            if (!result.IsSuccess)
                return View(new PagedResult<AdminTrackListItemDto>());

            ViewBag.Search = search;
            ViewBag.CourseId = courseId;
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var model = new CreateTrackViewModel
            {
                CourseOptions = await GetCourseOptionsAsync(ct),
                NarratorOptions = await GetNarratorOptionsAsync(ct)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrackViewModel model, CancellationToken ct)
        {
            var validationResult = await _createValidator.ValidateAsync(model, ct);
            if (!validationResult.IsValid)
            {
                foreach (var failure in validationResult.Errors)
                    ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);

                model.CourseOptions = await GetCourseOptionsAsync(ct);
                model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                return View(model);
            }

            await using var stream = model.AudioFile.OpenReadStream();
            var uploadResult = await Dispatcher.Send(
                new UploadFileCommand(stream, model.AudioFile.FileName, model.AudioFile.ContentType, StorageType.Audio),
                ct);

            if (!uploadResult.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, "Audio fayl yüklənərkən xəta baş verdi.");
                model.CourseOptions = await GetCourseOptionsAsync(ct);
                model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                return View(model);
            }

            var command = new CreateTrackCommand
            {
                DurationSec = model.DurationSec,
                AudioUrl = uploadResult.Value.FileName,
                MimeType = model.AudioFile.ContentType,
                FileSizeBytes = model.AudioFile.Length,
                CourseId = model.CourseId,
                NarratorId = model.NarratorId,
                Languages = model.Languages
            };

            var createResult = await Dispatcher.Send(command, ct);
            if (!createResult.IsSuccess)
            {
                AddResultErrorsToModelState(createResult.Error);
                model.CourseOptions = await GetCourseOptionsAsync(ct);
                model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id, CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetTrackByIdQuery { Id = id }, ct);
            if (!result.IsSuccess)
                return NotFound();

            var track = result.Value;

            var model = new EditTrackViewModel
            {
                Id = id,
                DurationSec = track.DurationSec,
                ExistingAudioUrl = track.AudioUrl,
                CourseId = track.CourseId,
                NarratorId = track.NarratorId,
                Languages = track.Languages,
                CourseOptions = await GetCourseOptionsAsync(ct),
                NarratorOptions = await GetNarratorOptionsAsync(ct)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditTrackViewModel model, CancellationToken ct)
        {
            var audioUrl = model.ExistingAudioUrl;
            var mimeType = string.Empty;
            var fileSizeBytes = 0L;
            var audioReplaced = false;

            if (model.AudioFile is not null)
            {
                await using var stream = model.AudioFile.OpenReadStream();
                var uploadResult = await Dispatcher.Send(
                    new UploadFileCommand(stream, model.AudioFile.FileName, model.AudioFile.ContentType, StorageType.Audio),
                    ct);

                if (!uploadResult.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, "Audio fayl yüklənərkən xəta baş verdi.");
                    model.CourseOptions = await GetCourseOptionsAsync(ct);
                    model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                    return View(model);
                }

                audioUrl = uploadResult.Value.FileName;
                mimeType = model.AudioFile.ContentType;
                fileSizeBytes = model.AudioFile.Length;
                audioReplaced = true;
            }

            var command = new UpdateTrackCommand
            {
                Id = model.Id,
                DurationSec = model.DurationSec,
                AudioUrl = audioUrl,
                MimeType = mimeType,
                FileSizeBytes = fileSizeBytes,
                CourseId = model.CourseId,
                NarratorId = model.NarratorId,
                AudioReplaced = audioReplaced,
                Languages = model.Languages
            };

            var updateResult = await Dispatcher.Send(command, ct);
            if (!updateResult.IsSuccess)
            {
                AddResultErrorsToModelState(updateResult.Error);
                model.CourseOptions = await GetCourseOptionsAsync(ct);
                model.NarratorOptions = await GetNarratorOptionsAsync(ct);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var deleteResult = await Dispatcher.Send(new DeleteTrackCommand { Id = id }, ct);
            if (!deleteResult.IsSuccess)
                TempData["Error"] = "Track silinərkən xəta baş verdi.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<CourseOptionDto>> GetCourseOptionsAsync(CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetCourseOptionsQuery(), ct);
            return result.IsSuccess ? result.Value : new List<CourseOptionDto>();
        }

        private async Task<List<NarratorOptionDto>> GetNarratorOptionsAsync(CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetNarratorOptionsQuery(), ct);
            return result.IsSuccess ? result.Value : new List<NarratorOptionDto>();
        }
    }
}
