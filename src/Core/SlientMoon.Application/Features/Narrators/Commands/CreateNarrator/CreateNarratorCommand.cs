using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Narrators.Commands.CreateNarrator
{
    public class CreateNarratorCommand : ICommand<string>
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
    }

    public class CreateNarratorCommandHandler : ICommandHandler<CreateNarratorCommand, string>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<CreateNarratorCommandHandler> _logger;

        public CreateNarratorCommandHandler(IUow uow, IAppLogger<CreateNarratorCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(CreateNarratorCommand command, CancellationToken ct)
        {
            _logger.LogInformation("CreateNarrator started. Name: {Name}", command.Name);

            var narrator = new Narrator
            {
                Name = command.Name,
                Gender = command.Gender
            };

            await _uow.NarratorRepository.AddAsync(narrator, ct);

            _logger.LogInformation("CreateNarrator completed. Id: {Id}", narrator.Id);

            return narrator.Id;
        }
    }
}
