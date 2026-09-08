using Application.Abstractions.Messaging;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Narrators.Commands.DeleteNarrator
{
    public class DeleteNarratorCommand : ICommand<bool>
    {
        public string Id { get; set; }
    }

    public class DeleteNarratorCommandHandler : ICommandHandler<DeleteNarratorCommand, bool>
    {
        private readonly IUow _uow;

        public DeleteNarratorCommandHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<bool>> Handle(DeleteNarratorCommand command, CancellationToken ct)
        {
            var existing = await _uow.NarratorRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return NarratorErrors.NarratorNotFound;

            var hasTracks = await _uow.NarratorRepository.HasTracksAsync(command.Id, ct);
            if (hasTracks)
                return NarratorErrors.NarratorHasTracks;

            _uow.NarratorRepository.Delete(existing);

            return true;
        }
    }
}
