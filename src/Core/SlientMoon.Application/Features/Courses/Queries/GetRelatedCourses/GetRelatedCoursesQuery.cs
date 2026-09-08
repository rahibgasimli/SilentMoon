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

namespace SlientMoon.Application.Features.Courses.Queries.GetRelatedCourses
{
    public record GetRelatedCoursesQuery(string Id, int Limit = 20) : IQuery<List<CourseDto>>
    {
    }

    public class GetRelatedCoursesQueryHandler : IQueryHandler<GetRelatedCoursesQuery, List<CourseDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetRelatedCoursesQueryHandler> _logger;
        private readonly ILanguageService _languageService;
        private readonly ITranslationService _translationService;

        public GetRelatedCoursesQueryHandler(
            IUow uow,
            IAppLogger<GetRelatedCoursesQueryHandler> logger,
            ILanguageService languageService,
            ITranslationService translationService)
        {
            _uow = uow;
            _logger = logger;
            _languageService = languageService;
            _translationService = translationService;
        }

        public async Task<Result<List<CourseDto>>> Handle(GetRelatedCoursesQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetRelatedCourses started. CourseId: {CourseId}, Limit: {Limit}", query.Id, query.Limit);

            LanguageCode currentLang = _languageService.GetCurrentLanguage();

            var currentCourse = await _uow.GenericRepository<Course>()
                .GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == query.Id, ct);

            if (currentCourse is null)
            {
                _logger.LogWarning("GetRelatedCourses failed. Course not found: {CourseId}", query.Id);
                return CourseErrors.NotFound;
            }

            var rows = await _uow.GenericRepository<Course>()
                .GetQueryable()
                .AsNoTracking()
                .Include(c => c.Category)
                    .ThenInclude(cat => cat.CategoryType)
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .Where(c => c.Id != query.Id && c.CategoryId == currentCourse.CategoryId)
                .Take(query.Limit)
                .Select(c => new { Course = c, TrackCount = c.Tracks.Count })
                .ToListAsync(ct);

            if (rows.Count < query.Limit)
            {
                var remainingLimit = query.Limit - rows.Count;
                var existingIds = rows.Select(r => r.Course.Id).ToList();
                existingIds.Add(query.Id);

                var fallbackRows = await _uow.GenericRepository<Course>()
                    .GetQueryable()
                    .AsNoTracking()
                    .Include(c => c.Category)
                        .ThenInclude(cat => cat.CategoryType)
                    .Include(c => c.CourseNarrators)
                        .ThenInclude(cn => cn.Narrator)
                    .Where(c => !existingIds.Contains(c.Id))
                    .Take(remainingLimit)
                    .Select(c => new { Course = c, TrackCount = c.Tracks.Count })
                    .ToListAsync(ct);

                rows.AddRange(fallbackRows);
            }

            Dictionary<string, string>? translations = null;
            if (currentLang != LanguageCode.EN && rows.Any())
            {
                var keys = rows.Select(r => r.Course).GetTranslationKeys(
                    nameof(Course.Title),
                    nameof(Course.Subtitle),
                    nameof(Course.Description)
                );

                translations = await _translationService.GetTranslationsAsync(keys, currentLang, ct);
            }

            var dtos = rows
                .Select(r => r.Course.ToHomeCourseDto(translations, r.TrackCount))
                .ToList();

            _logger.LogInformation("GetRelatedCourses completed. Returned {Count} items. Language: {Lang}", dtos.Count, currentLang);

            return dtos;
        }
    }
}
