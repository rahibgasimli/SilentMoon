using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.DTOs.Favorites;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Application.Mappings;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Favorites.Queries.GetMyFavorites
{
    public class GetMyFavoritesQuery : IQuery<PagedResult<FavoriteDto>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? Type { get; set; }


        public GetMyFavoritesQuery()
        {

        }

        public GetMyFavoritesQuery(int page = 1, int limit = 20, string? type = null)
        {
            Page = page < 1 ? 1 : page;
            Limit = limit < 1 ? 20 : limit;
            Type = type;
        }
    }

    public class GetMyFavoritesQueryHandler : IQueryHandler<GetMyFavoritesQuery, PagedResult<FavoriteDto>>
    {
        private readonly IUow _uow;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAppLogger<GetMyFavoritesQueryHandler> _logger;
        private readonly ILanguageService _languageService;
        private readonly ITranslationService _translationService;

        public GetMyFavoritesQueryHandler(
            IUow uow,
            ICurrentUserService currentUserService,
            IAppLogger<GetMyFavoritesQueryHandler> logger,
            ILanguageService languageService,
            ITranslationService translationService)
        {
            _uow = uow;
            _currentUserService = currentUserService;
            _logger = logger;
            _languageService = languageService;
            _translationService = translationService;
        }


        public async Task<Result<PagedResult<FavoriteDto>>> Handle(GetMyFavoritesQuery query, CancellationToken ct)
        {
            string userId = _currentUserService.GetUser();

            _logger.LogInformation("GetMyFavorites started. UserId: {UserId}, Page: {Page}, Limit: {Limit}, Type: {Type}",
                userId, query.Page, query.Limit, query.Type?.ToString() ?? "All");

            LanguageCode currentLang = _languageService.GetCurrentLanguage();

            var favoritesQuery = _uow.FavoriteRepository.GetQueryable()
                .Where(f => f.UserId == userId);


            if (!string.IsNullOrWhiteSpace(query.Type))
            {
                var normalizedType = query.Type.Trim().ToLower();
                favoritesQuery = favoritesQuery.Where(f => f.Course.Category.CategoryType.Slug.ToLower() == normalizedType);
            }


            int total = await favoritesQuery.CountAsync(ct);

            var rows = await favoritesQuery
                .OrderByDescending(f => f.CreatedAt)
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .Include(f => f.Course)
                    .ThenInclude(c => c.Category)
                        .ThenInclude(cat => cat.CategoryType)
                .Include(f => f.Course)
                    .ThenInclude(c => c.CourseNarrators)
                        .ThenInclude(cn => cn.Narrator)
                .Select(f => new
                {
                    Favorite = f,
                    TrackCount = f.Course.Tracks.Count
                })
                .AsNoTracking()
                .ToListAsync(ct);

            Dictionary<string, string>? translations = null;
            if (currentLang != LanguageCode.EN && rows.Any())
            {
                var keys = rows
                    .Where(r => r.Favorite.Course != null)
                    .Select(r => r.Favorite.Course)
                    .GetTranslationKeys(
                        nameof(Course.Title),
                        nameof(Course.Subtitle),
                        nameof(Course.Description)
                    );

                translations = await _translationService.GetTranslationsAsync(keys, currentLang, ct);
            }

            var items = rows
                .Select(r => new FavoriteDto
                {
                    Id = r.Favorite.Id,
                    CourseId = r.Favorite.CourseId,
                    CreatedAt = r.Favorite.CreatedAt,
                    Course = r.Favorite.Course?.ToHomeCourseDto(translations, r.TrackCount)
                })
                .ToList();

            int totalPages = (int)Math.Ceiling((double)total / query.Limit);

            _logger.LogInformation("GetMyFavorites completed. Returned {Count} items out of {Total}. Language: {Lang}", items.Count, total, currentLang);

            var pagedResult = new PagedResult<FavoriteDto>
            {
                Data = items,
                Meta = new PageMeta
                {
                    Page = query.Page,
                    Limit = query.Limit,
                    Total = total,
                    TotalPages = totalPages
                }
            };

            return pagedResult;
        }
    }
}
