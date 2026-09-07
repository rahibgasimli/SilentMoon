using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand : ICommand<string>
    {
        public string Slug { get; set; }
        public string IconUrl { get; set; }
        public string CategoryTypeId { get; set; }

        public Dictionary<LanguageCode, CategoryTranslationInput> Languages { get; set; }
    }

    public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, string>
    {
        private readonly IUow _uow;
        private readonly ITranslationService _translationService;
        private readonly IAppLogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(
            IUow uow,
            ITranslationService translationService,
            IAppLogger<CreateCategoryCommandHandler> logger)
        {
            _uow = uow;
            _translationService = translationService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(CreateCategoryCommand command, CancellationToken ct)
        {
            _logger.LogInformation("CreateCategory started. Name(EN): {Name}", command.Languages[LanguageCode.EN].Name);

            var category = new Category
            {
                Name = command.Languages[LanguageCode.EN].Name,
                Slug = command.Slug,
                IconUrl = command.IconUrl,
                CategoryTypeId = command.CategoryTypeId
            };

            await _uow.CategoryRepository.AddAsync(category, ct);

            await _translationService.SaveAllTranslationsAsync<Category, CategoryTranslationInput>(
                category.Id,
                command.Languages,
                t => new Dictionary<TranslatableField, string> { { TranslatableField.Name, t.Name } },
                ct);

            _logger.LogInformation("CreateCategory completed. CategoryId: {CategoryId}", category.Id);

            return category.Id;
        }
    }

}
