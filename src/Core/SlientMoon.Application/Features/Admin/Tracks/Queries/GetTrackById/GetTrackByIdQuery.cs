using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Tracks.Queries.GetTrackById
{
    public class GetTrackByIdQuery : IQuery<AdminTrackInput>
    {
        public string Id { get; set; }
    }

    public class GetTrackByIdAdminQueryHandler : IQueryHandler<GetTrackByIdQuery, AdminTrackInput>
    {
        private readonly IUow _uow;
        private readonly ITranslationService _translationService;

        public GetTrackByIdAdminQueryHandler(IUow uow, ITranslationService translationService)
        {
            _uow = uow;
            _translationService = translationService;
        }

        public async Task<Result<AdminTrackInput>> Handle(GetTrackByIdQuery query, CancellationToken ct)
        {
            var track = await _uow.TrackRepository.GetByIdAsync(query.Id, ct);
            if (track is null)
                return TrackErrors.TrackNotFound;

            var key = TranslationExtensions.BuildKey<Track>(TranslatableField.Title.ToString(), query.Id);
            var keys = new List<string> { key };

            var azTranslations = await _translationService.GetTranslationsAsync(keys, LanguageCode.AZ, ct);
            var ruTranslations = await _translationService.GetTranslationsAsync(keys, LanguageCode.RU, ct);

            azTranslations.TryGetValue(key, out var azTitle);
            ruTranslations.TryGetValue(key, out var ruTitle);

            var languages = new Dictionary<LanguageCode, AdminTrackTranslationInput>
            {
                [LanguageCode.EN] = new AdminTrackTranslationInput { Title = track.Title },
                [LanguageCode.AZ] = new AdminTrackTranslationInput { Title = azTitle },
                [LanguageCode.RU] = new AdminTrackTranslationInput { Title = ruTitle }
            };

            return new AdminTrackInput
            {
                DurationSec = track.DurationSec,
                AudioUrl = track.AudioUrl,
                MimeType = track.MimeType,
                FileSizeBytes = track.FileSizeBytes,
                CourseId = track.CourseId,
                NarratorId = track.NarratorId,
                Languages = languages
            };

        }
    }
}
