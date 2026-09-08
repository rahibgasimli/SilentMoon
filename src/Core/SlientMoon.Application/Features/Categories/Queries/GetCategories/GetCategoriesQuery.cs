using Microsoft.EntityFrameworkCore;
using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQuery : IQuery<List<CategoryDto>>
    {
        public string? Type { get; set; }

        public GetCategoriesQuery(string? type = null)
        {
            Type = type;
        }
    }

    public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IUow _uow;
        private readonly IAppLogger<GetCategoriesQueryHandler> _logger;
        private readonly ILanguageService _languageService;
        private readonly ITranslationService _translationService;

        public GetCategoriesQueryHandler(
            IUow uow,
            IAppLogger<GetCategoriesQueryHandler> logger,
            ILanguageService languageService,
            ITranslationService translationService)
        {
            _uow = uow;
            _logger = logger;
            _languageService = languageService;
            _translationService = translationService;
        }

        public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery query, CancellationToken ct)
        {
            _logger.LogInformation("GetCategories started. Filter Type: {Type}", query.Type?.ToString() ?? "All");

            LanguageCode currentLang = _languageService.GetCurrentLanguage();

            var categoryQuery = _uow.CategoryRepository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(query.Type))
            {
                var normalizedType = query.Type.Trim().ToLower();
                categoryQuery = categoryQuery.Where(c => c.CategoryType.Slug.ToLower() == normalizedType);
            }

            var categoryEntities = await categoryQuery
                .AsNoTracking()
                .Include(c => c.CategoryType)
                .ToListAsync(ct);

            Dictionary<string, string>? translations = null;
            if (currentLang != LanguageCode.EN && categoryEntities.Any())
            {
                var keys = categoryEntities.GetTranslationKeys(nameof(Category.Name));

                translations = await _translationService.GetTranslationsAsync(keys, currentLang, ct);
            }

            var categories = categoryEntities
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    Title = c.GetTranslation(x => x.Name, translations),
                    Type = c.CategoryType?.Name,
                    CategoryTypeId = c.CategoryTypeId,
                    IconUrl = c.IconUrl
                })
                .ToList();

            _logger.LogInformation("GetCategories completed. Count: {Count}. Language: {Lang}", categories.Count, currentLang);

            return categories;
        }
    }

}
