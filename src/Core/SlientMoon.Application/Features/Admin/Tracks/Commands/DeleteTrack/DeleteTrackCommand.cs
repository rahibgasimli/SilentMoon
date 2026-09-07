using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Tracks.Commands.DeleteTrack
{
    public class DeleteTrackCommand : ICommand<bool>
    {
        public string Id { get; set; }
    }

    public class DeleteTrackCommandHandler : ICommandHandler<DeleteTrackCommand, bool>
    {
        private static readonly TranslatableField[] Fields = { TranslatableField.Title };

        private readonly IUow _uow;
        private readonly ITranslationService _translationService;
        private readonly IStorageService _storageService;

        public DeleteTrackCommandHandler(
            IUow uow,
            ITranslationService translationService,
            IStorageService storageService)
        {
            _uow = uow;
            _translationService = translationService;
            _storageService = storageService;
        }

        public async Task<Result<bool>> Handle(DeleteTrackCommand command, CancellationToken ct)
        {
            var existing = await _uow.TrackRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return TrackErrors.TrackNotFound;

            if (!string.IsNullOrWhiteSpace(existing.AudioUrl))
                await _storageService.DeleteAsync(existing.AudioUrl, StorageType.Audio, ct);

            await _translationService.DeleteTranslationsAsync<Track>(command.Id, Fields, ct);

            _uow.TrackRepository.Delete(existing);

            return true;
        }
    }
}
