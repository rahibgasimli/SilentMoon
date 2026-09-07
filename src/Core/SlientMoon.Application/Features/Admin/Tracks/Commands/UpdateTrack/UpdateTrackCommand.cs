using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Tracks.Commands.UpdateTrack
{
    public class UpdateTrackCommand : ICommand<bool>
    {
        public string Id { get; set; }
        public int DurationSec { get; set; }
        public string AudioUrl { get; set; }
        public string MimeType { get; set; }
        public long FileSizeBytes { get; set; }
        public string CourseId { get; set; }
        public string NarratorId { get; set; }
        public bool AudioReplaced { get; set; }

        public Dictionary<LanguageCode, AdminTrackTranslationInput> Languages { get; set; }
    }

    public class UpdateTrackCommandHandler : ICommandHandler<UpdateTrackCommand, bool>
    {
        private readonly IUow _uow;
        private readonly ITranslationService _translationService;
        private readonly IStorageService _storageService;
        private readonly IAppLogger<UpdateTrackCommandHandler> _logger;

        public UpdateTrackCommandHandler(
            IUow uow,
            ITranslationService translationService,
            IStorageService storageService,
            IAppLogger<UpdateTrackCommandHandler> logger)
        {
            _uow = uow;
            _translationService = translationService;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(UpdateTrackCommand command, CancellationToken ct)
        {
            var existing = await _uow.TrackRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return TrackErrors.TrackNotFound;

            var oldAudioUrl = existing.AudioUrl;

            existing.DurationSec = command.DurationSec;
            existing.AudioUrl = command.AudioUrl;

            if (command.AudioReplaced)
            {
                existing.MimeType = command.MimeType;
                existing.FileSizeBytes = command.FileSizeBytes;
            }
            existing.CourseId = command.CourseId;
            existing.NarratorId = command.NarratorId;

            if (command.Languages.TryGetValue(LanguageCode.EN, out var en) && !string.IsNullOrWhiteSpace(en.Title))
                existing.Title = en.Title;

            _uow.TrackRepository.Update(existing);

            await _translationService.SaveAllTranslationsAsync<Track, AdminTrackTranslationInput>(
                command.Id,
                command.Languages,
                t => new Dictionary<TranslatableField, string> { { TranslatableField.Title, t.Title } },
                ct);


            if (command.AudioReplaced && !string.IsNullOrWhiteSpace(oldAudioUrl) && oldAudioUrl != command.AudioUrl)
            {
                await _storageService.DeleteAsync(oldAudioUrl, StorageType.Audio, ct);
            }


            _logger.LogInformation("UpdateTrack completed. TrackId: {TrackId}", command.Id);

            return true;
        }
    }
}
