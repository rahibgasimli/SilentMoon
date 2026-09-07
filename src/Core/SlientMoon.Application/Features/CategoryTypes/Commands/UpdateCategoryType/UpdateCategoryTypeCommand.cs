using Application.Abstractions.Messaging;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.CategoryTypes.Commands.UpdateCategoryType
{
    public class UpdateCategoryTypeCommand : ICommand<bool>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    public class UpdateCategoryTypeCommandHandler : ICommandHandler<UpdateCategoryTypeCommand, bool>
    {
        private readonly IUow _uow;

        public UpdateCategoryTypeCommandHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<bool>> Handle(UpdateCategoryTypeCommand command, CancellationToken ct)
        {
            var existing = await _uow.CategoryTypeRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return CategoryTypeErrors.CategoryTypeNotFound;

            existing.Name = command.Name;
            existing.Slug = command.Slug;

            _uow.CategoryTypeRepository.Update(existing);

            return true;
        }
    }
}