using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Application.Mappings;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Courses.Queries.GetCourses
{
    public class GetCoursesQuery : IQuery<PagedResult<CourseDto>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string Sort { get; set; } = "createdAt_desc";
        public string? Type { get; set; }
        public string? CategoryId { get; set; }
        public string? Q { get; set; }
        public bool? IsFeatured { get; set; }
    }


    public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, PagedResult<CourseDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetCoursesQueryHandler> _logger;
        private readonly ILanguageService _languageService;
        private readonly ITranslationService _translationService;

        public GetCoursesQueryHandler(
            IUow uow,
            IAppLogger<GetCoursesQueryHandler> logger,
            ILanguageService languageService,
            ITranslationService translationService)
        {
            _uow = uow;
            _logger = logger;
            _languageService = languageService;
            _translationService = translationService;
        }

        public async Task<Result<PagedResult<CourseDto>>> Handle(GetCoursesQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetCourses started. Page: {Page}, Limit: {Limit}, Sort: {Sort}", query.Page, query.Limit, query.Sort);

            LanguageCode currentLang = _languageService.GetCurrentLanguage();

            var coursesQuery = _uow.CourseRepository.GetQueryable();


            if (!string.IsNullOrWhiteSpace(query.Type))
            {
                var normalizedType = query.Type.Trim().ToLower();
                coursesQuery = coursesQuery.Where(c => c.Category.CategoryType.Slug.ToLower() == normalizedType);
            }

            if (!string.IsNullOrWhiteSpace(query.CategoryId))
            {
                coursesQuery = coursesQuery.Where(c => c.CategoryId == query.CategoryId);
            }

            if (query.IsFeatured.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.IsFeatured == query.IsFeatured.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Q))
            {
                var search = query.Q.Trim().ToLower();
                coursesQuery = coursesQuery.Where(c => c.Title.ToLower().Contains(search) || c.Subtitle.ToLower().Contains(search));
            }

            int total = await coursesQuery.CountAsync(ct);

            coursesQuery = query.Sort?.ToLower() switch
            {
                "createdat_asc" => coursesQuery.OrderBy(c => c.CreatedAt),
                "title_asc" => coursesQuery.OrderBy(c => c.Title),
                "popular" => coursesQuery.OrderByDescending(c => c.ViewCount),
                _ => coursesQuery.OrderByDescending(c => c.CreatedAt)
            };

            var rows = await coursesQuery
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .Include(c => c.Category)
                    .ThenInclude(cat => cat.CategoryType)
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .Select(c => new { Course = c, TrackCount = c.Tracks.Count })
                .AsNoTracking()
                .ToListAsync(ct);

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

            var items = rows
                           .Select(r => r.Course.ToHomeCourseDto(translations, r.TrackCount))
                           .ToList();

            int totalPages = (int)Math.Ceiling((double)total / query.Limit);

            _logger.LogInformation("GetCourses completed. Returned {Count} items out of {Total}. Language: {Lang}", items.Count, total, currentLang);

            return new PagedResult<CourseDto>
            {
                Data = items,
                Meta = new PageMeta
                {
                    Page = query.Page,
                    Limit = query.Limit,
                    Total = total,
                    TotalPages = totalPages
                }
            };
        }
    }
}
