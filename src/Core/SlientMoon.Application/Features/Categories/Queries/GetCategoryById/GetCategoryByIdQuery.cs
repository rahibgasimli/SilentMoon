using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQuery : IQuery<CategoryInput>
    {
        public string Id { get; set; }
    }

    public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, CategoryInput>
    {
        private readonly IUow _uow;
        private readonly ITranslationService _translationService;

        public GetCategoryByIdQueryHandler(IUow uow, ITranslationService translationService)
        {
            _uow = uow;
            _translationService = translationService;
        }

        public async Task<Result<CategoryInput>> Handle(GetCategoryByIdQuery query, CancellationToken ct)
        {
            var category = await _uow.CategoryRepository.GetByIdAsync(query.Id, ct);
            if (category is null)
                return CategoryErrors.CategoryNotFound;

            var key = TranslationExtensions.BuildKey<Category>(TranslatableField.Name.ToString(), query.Id);
            var keys = new List<string> { key };

            var azTranslations = await _translationService.GetTranslationsAsync(keys, LanguageCode.AZ, ct);
            var ruTranslations = await _translationService.GetTranslationsAsync(keys, LanguageCode.RU, ct);

            azTranslations.TryGetValue(key, out var azName);
            ruTranslations.TryGetValue(key, out var ruName);

            var languages = new Dictionary<LanguageCode, CategoryTranslationInput>
            {
                [LanguageCode.EN] = new CategoryTranslationInput { Name = category.Name },
                [LanguageCode.AZ] = new CategoryTranslationInput { Name = azName },
                [LanguageCode.RU] = new CategoryTranslationInput { Name = ruName }
            };

            return new CategoryInput
            {
                Slug = category.Slug,
                IconUrl = category.IconUrl,
                CategoryTypeId = category.CategoryTypeId,
                Languages = languages
            };
        }
    }
}
