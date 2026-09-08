using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Constants;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
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

            var languages = await _translationService.GetTranslationMapAsync<Track>(
                query.Id, TranslatableFields.Track, ct);

            languages.Set(LanguageCode.EN, TranslatableField.Title, track.Title);

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
