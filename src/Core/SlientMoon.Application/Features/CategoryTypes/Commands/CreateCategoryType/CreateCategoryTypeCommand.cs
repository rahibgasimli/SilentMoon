using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.CategoryTypes.Commands.CreateCategoryType
{
    public class CreateCategoryTypeCommand : ICommand<string>
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    public class CreateCategoryTypeCommandHandler : ICommandHandler<CreateCategoryTypeCommand, string>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<CreateCategoryTypeCommandHandler> _logger;

        public CreateCategoryTypeCommandHandler(IUow uow, IAppLogger<CreateCategoryTypeCommandHandler> logger)
        {
            _uow = uow;
            _logger = logger;
        }


        public async Task<Result<string>> Handle(CreateCategoryTypeCommand command, CancellationToken ct)
        {
            _logger.LogInformation("CreateCategoryType started. Name: {Name}", command.Name);

            var categoryType = new CategoryType
            {
                Name = command.Name,
                Slug = command.Slug
            };

            await _uow.CategoryTypeRepository.AddAsync(categoryType, ct);

            _logger.LogInformation("CreateCategoryType completed. Id: {Id}", categoryType.Id);

            return categoryType.Id;
        }
    }
}
