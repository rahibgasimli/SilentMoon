using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.DTOs.Narrators;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Narrators.Queries.GetNarrators
{
    public class GetNarratorsQuery : IQuery<PagedResult<NarratorListItemDto>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetNarratorsQueryHandler : IQueryHandler<GetNarratorsQuery, PagedResult<NarratorListItemDto>>
    {
        private readonly IUow _uow;

        public GetNarratorsQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<PagedResult<NarratorListItemDto>>> Handle(GetNarratorsQuery query, CancellationToken ct)
        {
            return await _uow.NarratorRepository.GetPagedAsync(query.Page, query.Limit, query.Search, ct);
        }
    }
}
