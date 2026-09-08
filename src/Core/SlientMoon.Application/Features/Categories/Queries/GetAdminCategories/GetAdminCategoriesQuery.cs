using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.DTOs.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Categories.Queries.GetAdminCategories
{
    public class GetAdminCategoriesQuery : IQuery<PagedResult<CategoryListItemDto>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? Search { get; set; }
        public string? CategoryTypeId { get; set; }
    }


    public class GetAdminCategoriesQueryHandler : IQueryHandler<GetAdminCategoriesQuery, PagedResult<CategoryListItemDto>>
    {
        private readonly IUow _uow;

        public GetAdminCategoriesQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<PagedResult<CategoryListItemDto>>> Handle(GetAdminCategoriesQuery query, CancellationToken ct)
        {
            return await _uow.CategoryRepository.GetPagedAsync(query.Page, query.Limit, query.Search, query.CategoryTypeId, ct);
        }
    }
}
