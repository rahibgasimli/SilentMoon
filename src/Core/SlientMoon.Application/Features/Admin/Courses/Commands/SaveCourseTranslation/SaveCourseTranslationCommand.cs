using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Courses.Commands.SaveCourseTranslation
{
    public class SaveCourseTranslationCommand : ICommand<bool>
    {
        public string CourseId { get; set; }
        public LanguageCode LanguageCode { get; set; }
        public string Title { get; set; } 
        public string Subtitle { get; set; }
        public string Description { get; set; }
    }

    public class SaveCourseTranslationCommandHandler : ICommandHandler<SaveCourseTranslationCommand, bool>
    {
        private readonly ITranslationService _translationService;

        public SaveCourseTranslationCommandHandler(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        public async Task<Result<bool>> Handle(SaveCourseTranslationCommand command, CancellationToken ct)
        {
            var propertyValues = new Dictionary<TranslatableField, string>
            {
                { TranslatableField.Title, command.Title },
                { TranslatableField.Subtitle, command.Subtitle },
                { TranslatableField.Description, command.Description }
            };

            await _translationService.SaveTranslationsAsync<Course>(
                command.CourseId,
                command.LanguageCode,
                propertyValues,
                ct);

            return true;
        }
    }
}
