using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommand : ICommand<bool>
    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryId { get; set; }

        public AdminCourseTranslationInput En { get; set; }
        public AdminCourseTranslationInput Az { get; set; }
        public AdminCourseTranslationInput Ru { get; set; }
    }

    public class UpdateCourseCommandHandler : ICommandHandler<UpdateCourseCommand, bool>
    {
        private readonly IAdminCourseRepository _adminCourseRepository;
        private readonly ITranslationService _translationService;
        private readonly IAppLogger<UpdateCourseCommandHandler> _logger;

        public UpdateCourseCommandHandler(
            IAdminCourseRepository adminCourseRepository,
            ITranslationService translationService,
            IAppLogger<UpdateCourseCommandHandler> logger)
        {
            _adminCourseRepository = adminCourseRepository;
            _translationService = translationService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(UpdateCourseCommand command, CancellationToken ct)
        {
            var existing = await _adminCourseRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return CourseErrors.NotFound;

            var input = new AdminCourseInput
            {
                ImageUrl = command.ImageUrl,
                DurationSec = command.DurationSec,
                IsFeatured = command.IsFeatured,
                CategoryId = command.CategoryId,
                En = command.En,
                Az = command.Az,
                Ru = command.Ru
            };

            await _adminCourseRepository.UpdateCourseAsync(command.Id, input, ct);

            await SaveTranslationIfPresentAsync(command.Id, LanguageCode.EN, command.En, ct);
            await SaveTranslationIfPresentAsync(command.Id, LanguageCode.AZ, command.Az, ct);
            await SaveTranslationIfPresentAsync(command.Id, LanguageCode.RU, command.Ru, ct);

            _logger.LogInformation("UpdateCourse completed. CourseId: {CourseId}", command.Id);

            return true;

        }

        private async Task SaveTranslationIfPresentAsync(
    string courseId, LanguageCode languageCode, AdminCourseTranslationInput translation, CancellationToken ct)
        {
            if (translation is null)
                return;

            var propertyValues = new Dictionary<TranslatableField, string>();

            if (!string.IsNullOrWhiteSpace(translation.Title))
                propertyValues[TranslatableField.Title] = translation.Title;

            if (!string.IsNullOrWhiteSpace(translation.Subtitle))
                propertyValues[TranslatableField.Subtitle] = translation.Subtitle;

            if (!string.IsNullOrWhiteSpace(translation.Description))
                propertyValues[TranslatableField.Description] = translation.Description;

            if (propertyValues.Count == 0)
                return;

            await _translationService.SaveTranslationsAsync<Course>(courseId, languageCode, propertyValues, ct);
        }
    }
}
