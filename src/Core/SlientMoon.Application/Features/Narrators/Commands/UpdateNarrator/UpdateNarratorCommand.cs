using Application.Abstractions.Messaging;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Narrators.Commands.UpdateNarrator
{
    public class UpdateNarratorCommand : ICommand<bool>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
    }

    public class UpdateNarratorCommandHandler : ICommandHandler<UpdateNarratorCommand, bool>
    {
        private readonly IUow _uow;

        public UpdateNarratorCommandHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<bool>> Handle(UpdateNarratorCommand command, CancellationToken ct)
        {
            var existing = await _uow.NarratorRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return NarratorErrors.NarratorNotFound;

            existing.Name = command.Name;
            existing.Gender = command.Gender;

            _uow.NarratorRepository.Update(existing);

            return true;
        }
    }
}
