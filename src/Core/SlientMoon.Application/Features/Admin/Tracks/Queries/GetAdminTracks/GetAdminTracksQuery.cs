using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.DTOs.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Tracks.Queries.GetAdminTracks
{
    public class GetAdminTracksQuery : IQuery<PagedResult<AdminTrackListItemDto>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? Search { get; set; }
        public string? CourseId { get; set; }
    }

    public class GetAdminTracksQueryHandler : IQueryHandler<GetAdminTracksQuery, PagedResult<AdminTrackListItemDto>>
    {
        private readonly IUow _uow;

        public GetAdminTracksQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<PagedResult<AdminTrackListItemDto>>> Handle(GetAdminTracksQuery query, CancellationToken ct)
        {
            return await _uow.TrackRepository.GetPagedAsync(query.Page, query.Limit, query.Search, query.CourseId, ct);
        }
    }
}

