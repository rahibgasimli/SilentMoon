using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Constants;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Courses.Commands.DeleteCourse
{
    public class DeleteCourseCommand : ICommand<bool>
    {
        public string Id { get; set; }
    }

    public class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand, bool>
    {
        private readonly IAdminCourseRepository _adminCourseRepository;
        private readonly ITranslationService _translationService;

        public DeleteCourseCommandHandler(
            IAdminCourseRepository adminCourseRepository,
            ITranslationService translationService)
        {
            _adminCourseRepository = adminCourseRepository;
            _translationService = translationService;
        }

        public async Task<Result<bool>> Handle(DeleteCourseCommand command, CancellationToken ct)
        {
            var existing = await _adminCourseRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return CourseErrors.NotFound;

            await _translationService.DeleteTranslationsAsync<Course>(command.Id, TranslatableFields.Course, ct);
            await _adminCourseRepository.DeleteCourseAsync(command.Id, ct);

            return true;
        }
    }
}
