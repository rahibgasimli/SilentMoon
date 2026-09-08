using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommand : ICommand<bool>
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public string IconUrl { get; set; }
        public string CategoryTypeId { get; set; }
        public bool IconReplaced { get; set; }

        public Dictionary<LanguageCode, Dictionary<TranslatableField, string>> Languages { get; set; } = new();
    }

    public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, bool>
    {
        private readonly IUow _uow;
        private readonly ITranslationService _translationService;
        private readonly IStorageService _storageService;
        private readonly IAppLogger<UpdateCategoryCommandHandler> _logger;

        public UpdateCategoryCommandHandler(
            IUow uow,
            ITranslationService translationService,
            IStorageService storageService,
            IAppLogger<UpdateCategoryCommandHandler> logger)
        {
            _uow = uow;
            _translationService = translationService;
            _storageService = storageService;
            _logger = logger;
        }


        public async Task<Result<bool>> Handle(UpdateCategoryCommand command, CancellationToken ct)
        {
            var existing = await _uow.CategoryRepository.GetByIdAsync(command.Id, ct);
            if (existing is null)
                return CategoryErrors.CategoryNotFound;

            var oldIconUrl = existing.IconUrl;

            existing.Slug = command.Slug;
            existing.CategoryTypeId = command.CategoryTypeId;

            if (command.IconReplaced)
            {
                existing.IconUrl = command.IconUrl;
            }


            var nameEn = command.Languages.Get(LanguageCode.EN, TranslatableField.Name);
            if (!string.IsNullOrWhiteSpace(nameEn))
                existing.Name = nameEn;

            _uow.CategoryRepository.Update(existing);

            await _translationService.SaveAllTranslationsAsync<Category>(command.Id, command.Languages, ct);

            if (command.IconReplaced && !string.IsNullOrWhiteSpace(oldIconUrl) && oldIconUrl != command.IconUrl)
            {
                await _storageService.DeleteAsync(oldIconUrl, StorageType.Image, ct);
            }

            _logger.LogInformation("UpdateCategory completed. CategoryId: {CategoryId}", command.Id);

            return true;
        }
    }
}
