using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Tracks;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Courses.Queries.GetCourseTracks
{
    public record GetCourseTracksQuery(string CourseId, Gender? NarratorGender) : IQuery<List<TrackDetailDto>>;

    public class GetCourseTracksQueryHandler : IQueryHandler<GetCourseTracksQuery, List<TrackDetailDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetCourseTracksQueryHandler> _logger;
        private readonly ILanguageService _languageService;
        private readonly ITranslationService _translationService;

        public GetCourseTracksQueryHandler(
            IUow uow,
            IAppLogger<GetCourseTracksQueryHandler> logger,
            ILanguageService languageService,
            ITranslationService translationService)
        {
            _uow = uow;
            _logger = logger;
            _languageService = languageService;
            _translationService = translationService;
        }

        public async Task<Result<List<TrackDetailDto>>> Handle(GetCourseTracksQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetCourseTracks started. CourseId: {CourseId}, GenderFilter: {Narrator}", query.CourseId, query.NarratorGender);

            LanguageCode currentLang = _languageService.GetCurrentLanguage();

            var courseExists = await _uow.GenericRepository<Course>()
                .GetQueryable()
                .AsNoTracking()
                .AnyAsync(c => c.Id == query.CourseId, ct);


            if (!courseExists)
            {
                _logger.LogWarning("GetCourseTracks failed. Course not found: {CourseId}", query.CourseId);
                return CourseErrors.NotFound;
            }

            var tracksQuery = _uow.GenericRepository<Track>()
                .GetQueryable()
                .AsNoTracking()
                .Include(t => t.Narrator)
                .Where(t => t.CourseId == query.CourseId);

            if (query.NarratorGender.HasValue)
            {
                tracksQuery = tracksQuery.Where(t => t.Narrator != null && t.Narrator.Gender == query.NarratorGender.Value);
            }

            var tracks = await tracksQuery.ToListAsync(ct);

            Dictionary<string, string>? translations = null;
            if (currentLang != LanguageCode.EN && tracks.Any())
            {
                var keys = tracks.GetTranslationKeys(nameof(Track.Title));

                translations = await _translationService.GetTranslationsAsync(keys, currentLang, ct);
            }

            var dtos = tracks
                .Select(t => new TrackDetailDto
                {
                    Id = t.Id,
                    CourseId = t.CourseId,
                    Title = t.GetTranslation(x => x.Title, translations),
                    Narrator = t.Narrator != null ? t.Narrator.Gender.ToString().ToLower() : string.Empty,
                    DurationSec = t.DurationSec,
                    AudioUrl = t.AudioUrl,
                    MimeType = t.MimeType,
                    FileSizeBytes = t.FileSizeBytes,
                })
                .ToList();

            _logger.LogInformation("GetCourseTracks completed. Returned {Count} tracks. Language: {Lang}", dtos.Count, currentLang);

            return dtos;
        }
    }

}
