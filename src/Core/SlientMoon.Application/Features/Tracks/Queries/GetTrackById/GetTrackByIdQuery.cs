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
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Tracks.Queries.GetTrackById
{
    public class GetTrackByIdQuery : IQuery<TrackDetailDto>
    {
        public string Id { get; set; }

        public GetTrackByIdQuery(string id)
        {
            Id = id;
        }
    }

    public class GetTrackByIdQueryHandler : IQueryHandler<GetTrackByIdQuery, TrackDetailDto>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetTrackByIdQueryHandler> _logger;
        private readonly ILanguageService _languageService;
        private readonly ITranslationService _translationService;

        public GetTrackByIdQueryHandler(
            IUow uow,
            IAppLogger<GetTrackByIdQueryHandler> logger,
            ILanguageService languageService,
            ITranslationService translationService)
        {
            _uow = uow;
            _logger = logger;
            _languageService = languageService;
            _translationService = translationService;
        }

        public async Task<Result<TrackDetailDto>> Handle(GetTrackByIdQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetTrackById started. TrackId: {TrackId}", query.Id);

            LanguageCode currentLang = _languageService.GetCurrentLanguage();

            var track = await _uow.GenericRepository<Track>()
                .GetQueryable()
                .AsNoTracking()
                .Include(t => t.Narrator)
                .FirstOrDefaultAsync(t => t.Id == query.Id, ct);

            if (track is null)
            {
                _logger.LogWarning("GetTrackById failed. Track not found: {TrackId}", query.Id);
                return TrackErrors.TrackNotFound;
            }

            Dictionary<string, string>? translations = null;
            if (currentLang != LanguageCode.EN)
            {
                var keys = new[] { track }.GetTranslationKeys(nameof(Track.Title));

                translations = await _translationService.GetTranslationsAsync(keys, currentLang, ct);
            }

            var dto = new TrackDetailDto
            {
                Id = track.Id,
                CourseId = track.CourseId,
                Title = track.GetTranslation(t => t.Title, translations),
                Narrator = track.Narrator?.Gender.ToString().ToLower() ?? string.Empty,
                DurationSec = track.DurationSec,
                AudioUrl = track.AudioUrl,
                MimeType = track.MimeType,
                FileSizeBytes = track.FileSizeBytes,
            };

            return dto;
        }
    }

}
