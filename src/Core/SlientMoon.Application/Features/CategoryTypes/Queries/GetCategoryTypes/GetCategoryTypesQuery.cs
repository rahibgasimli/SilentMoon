using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs;
using SlientMoon.Application.DTOs.CategoryType;
using SlientMoon.Application.DTOs.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.CategoryTypes.Queries.GetCategoryTypes
{
    public class GetCategoryTypesQuery : IQuery<PagedResult<CategoryTypeListItemDto>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetCategoryTypesQueryHandler : IQueryHandler<GetCategoryTypesQuery, PagedResult<CategoryTypeListItemDto>>
    {
        private readonly IUow _uow;

        public GetCategoryTypesQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<PagedResult<CategoryTypeListItemDto>>> Handle(GetCategoryTypesQuery query, CancellationToken ct)
        {
            return await _uow.CategoryTypeRepository.GetPagedAsync(query.Page, query.Limit, query.Search, ct);
        }
    }
}