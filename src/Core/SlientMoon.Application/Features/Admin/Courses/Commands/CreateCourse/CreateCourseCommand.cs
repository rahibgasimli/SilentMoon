using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Courses.Commands.CreateCourse
{
    public class CreateCourseCommand : ICommand<string>
    {
        public string ImageUrl { get; set; }
        public int DurationSec { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryId { get; set; }

        public AdminCourseTranslationInput En { get; set; }
        public AdminCourseTranslationInput Az { get; set; }
        public AdminCourseTranslationInput Ru { get; set; }
    }

    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, string>
    {
        private readonly IAdminCourseRepository _adminCourseRepository;
        private readonly ITranslationService _translationService;
        private readonly IAppLogger<CreateCourseCommandHandler> _logger;

        public CreateCourseCommandHandler(
            IAdminCourseRepository adminCourseRepository,
            ITranslationService translationService,
            IAppLogger<CreateCourseCommandHandler> logger)
        {
            _adminCourseRepository = adminCourseRepository;
            _translationService = translationService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(CreateCourseCommand command, CancellationToken ct)
        {
            _logger.LogInformation("CreateCourse started. Title(EN): {Title}", command.En.Title);

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

            var courseId = await _adminCourseRepository.InsertCourseAsync(input, ct);

            await SaveTranslationIfPresentAsync(courseId, LanguageCode.EN, command.En, ct);
            await SaveTranslationIfPresentAsync(courseId, LanguageCode.AZ, command.Az, ct);
            await SaveTranslationIfPresentAsync(courseId, LanguageCode.RU, command.Ru, ct);

            _logger.LogInformation("CreateCourse completed. CourseId: {CourseId}", courseId);

            return courseId;
        }

        private async Task SaveTranslationIfPresentAsync(
            string courseId, LanguageCode languageCode, AdminCourseTranslationInput translation, CancellationToken ct)
        {
            if (translation is null)
                return;

            var propertyValues = new Dictionary<TranslatableField, string>
            {
                { TranslatableField.Title, translation.Title },
                { TranslatableField.Subtitle, translation.Subtitle },
                { TranslatableField.Description, translation.Description }
            };

            await _translationService.SaveTranslationsAsync<Course>(courseId, languageCode, propertyValues, ct);
        }
    }
}
