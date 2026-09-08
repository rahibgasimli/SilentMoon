using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Application.Mappings;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQuery : IQuery<CourseDto>
    {
        public string Id { get; set; }

        public GetCourseByIdQuery(string id)
        {
            Id = id;
        }
    }


    public class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetCourseByIdQueryHandler> _logger;
        private readonly ILanguageService _languageService;
        private readonly ITranslationService _translationService;

        public GetCourseByIdQueryHandler(
            IUow uow,
            IAppLogger<GetCourseByIdQueryHandler> logger,
            ILanguageService languageService,
            ITranslationService translationService)
        {
            _uow = uow;
            _logger = logger;
            _languageService = languageService;
            _translationService = translationService;
        }

        public async Task<Result<CourseDto>> Handle(GetCourseByIdQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetCourseById started. CourseId: {CourseId}", query.Id);

            LanguageCode currentLang = _languageService.GetCurrentLanguage();

            var row = await _uow.GenericRepository<Course>()
                .GetQueryable()
                .AsNoTracking()
                .Include(c => c.Category)
                    .ThenInclude(cat => cat.CategoryType)
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .Where(c => c.Id == query.Id)
                .Select(c => new { Course = c, TrackCount = c.Tracks.Count })
                .FirstOrDefaultAsync(ct);

            if (row is null)
            {
                _logger.LogWarning("GetCourseById failed. Course not found: {CourseId}", query.Id);
                return CourseErrors.NotFound;
            }

            Dictionary<string, string>? translations = null;
            if (currentLang != LanguageCode.EN)
            {
                var keys = new[] { row.Course }.GetTranslationKeys(
                    nameof(Course.Title),
                    nameof(Course.Subtitle),
                    nameof(Course.Description)
                );

                translations = await _translationService.GetTranslationsAsync(keys, currentLang, ct);
            }

            return row.Course.ToHomeCourseDto(translations, row.TrackCount);
        }
    }
}
