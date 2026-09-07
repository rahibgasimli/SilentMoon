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

        public Dictionary<LanguageCode, AdminCourseTranslationInput> Languages { get; set; }

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
            _logger.LogInformation("CreateCourse started. Title(EN): {Title}", command.Languages[LanguageCode.EN].Title);

            var input = new AdminCourseInput
            {
                ImageUrl = command.ImageUrl,
                DurationSec = command.DurationSec,
                IsFeatured = command.IsFeatured,
                CategoryId = command.CategoryId,
                Languages = command.Languages,
            };

            var courseId = await _adminCourseRepository.InsertCourseAsync(input, ct);


            await _translationService.SaveAllTranslationsAsync<Course, AdminCourseTranslationInput>(
                courseId,
                command.Languages,
                c => new Dictionary<TranslatableField, string>
                {
                    { TranslatableField.Title, c.Title },
                    { TranslatableField.Subtitle, c.Subtitle },
                    { TranslatableField.Description, c.Description }
                },
                ct);


            _logger.LogInformation("CreateCourse completed. CourseId: {CourseId}", courseId);

            return courseId;
        }

    }
}
