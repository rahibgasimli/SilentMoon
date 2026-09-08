using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Constants;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
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

            var languages = await _translationService.GetTranslationMapAsync<Category>(
                query.Id, TranslatableFields.Category, ct);

            languages.Set(LanguageCode.EN, TranslatableField.Name, category.Name);

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
