using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Tracks.Commands.CreateTrack
{
    public class CreateTrackCommand : ICommand<string>
    {
        public int DurationSec { get; set; }
        public string AudioUrl { get; set; }
        public string MimeType { get; set; }
        public long FileSizeBytes { get; set; }
        public string CourseId { get; set; }
        public string NarratorId { get; set; }

        public Dictionary<LanguageCode, Dictionary<TranslatableField, string>> Languages { get; set; } = new();
    }

    public class CreateTrackCommandHandler : ICommandHandler<CreateTrackCommand, string>
    {
        private readonly IUow _uow;
        private readonly ITranslationService _translationService;
        private readonly IAppLogger<CreateTrackCommandHandler> _logger;

        public CreateTrackCommandHandler(
            IUow uow,
            ITranslationService translationService,
            IAppLogger<CreateTrackCommandHandler> logger)
        {
            _uow = uow;
            _translationService = translationService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(CreateTrackCommand command, CancellationToken ct)
        {
            var titleEn = command.Languages.Get(LanguageCode.EN, TranslatableField.Title);

            _logger.LogInformation("CreateTrack started. Title(EN): {Title}", titleEn);

            var track = new Track
            {
                Title = titleEn,
                DurationSec = command.DurationSec,
                AudioUrl = command.AudioUrl,
                MimeType = command.MimeType,
                FileSizeBytes = command.FileSizeBytes,
                CourseId = command.CourseId,
                NarratorId = command.NarratorId
            };

            await _uow.TrackRepository.AddAsync(track, ct);

            await _translationService.SaveAllTranslationsAsync<Track>(track.Id, command.Languages, ct);

            _logger.LogInformation("CreateTrack completed. TrackId: {TrackId}", track.Id);

            return track.Id;
        }
    }
}
