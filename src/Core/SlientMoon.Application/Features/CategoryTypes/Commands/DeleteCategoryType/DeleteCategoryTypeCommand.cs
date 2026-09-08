using Application.Abstractions.Messaging;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.CategoryTypes.Commands.DeleteCategoryType
{
    public class DeleteCategoryTypeCommand : ICommand<bool>
    {
        public string Id { get; set; }
    }

    public class DeleteCategoryTypeCommandHandler : ICommandHandler<DeleteCategoryTypeCommand, bool>
    {
        private readonly IUow _uow;

        public DeleteCategoryTypeCommandHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<bool>> Handle(DeleteCategoryTypeCommand command, CancellationToken ct)
        {
            var existing = await _uow.CategoryTypeRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return CategoryTypeErrors.CategoryTypeNotFound;

            _uow.CategoryTypeRepository.Delete(existing);

            return true;
        }
    }
}