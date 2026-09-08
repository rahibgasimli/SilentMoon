using Application.Abstractions.Messaging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Constants;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommand : ICommand<bool>
    {
        public string Id { get; set; }
    }

    public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, bool>
    {
        private readonly IUow _uow;
        private readonly ITranslationService _translationService;
        private readonly IStorageService _storageService;

        public DeleteCategoryCommandHandler(
            IUow uow,
            ITranslationService translationService,
            IStorageService storageService)
        {
            _uow = uow;
            _translationService = translationService;
            _storageService = storageService;
        }

        public async Task<Result<bool>> Handle(DeleteCategoryCommand command, CancellationToken ct)
        {
            var existing = await _uow.CategoryRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return CategoryErrors.CategoryNotFound;

            if (!string.IsNullOrWhiteSpace(existing.IconUrl))
                await _storageService.DeleteAsync(existing.IconUrl, StorageType.Image, ct);

            await _translationService.DeleteTranslationsAsync<Category>(command.Id, TranslatableFields.Category, ct);

            _uow.CategoryRepository.Delete(existing);

            return true;
        }
    }
}
