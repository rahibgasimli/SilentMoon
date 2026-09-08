using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
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

        public Dictionary<LanguageCode, Dictionary<TranslatableField, string>> Languages { get; set; } = new();
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
            var nameEn = command.Languages.Get(LanguageCode.EN, TranslatableField.Name);

            _logger.LogInformation("CreateCategory started. Name(EN): {Name}", nameEn);

            var category = new Category
            {
                Name = nameEn,
                Slug = command.Slug,
                IconUrl = command.IconUrl,
                CategoryTypeId = command.CategoryTypeId
            };

            await _uow.CategoryRepository.AddAsync(category, ct);

            await _translationService.SaveAllTranslationsAsync<Category>(category.Id, command.Languages, ct);

            _logger.LogInformation("CreateCategory completed. CategoryId: {CategoryId}", category.Id);

            return category.Id;
        }
    }

}
