using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Admin;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Tracks.Queries.GetNarratorOptions
{
    public class GetNarratorOptionsQuery : IQuery<List<NarratorOptionDto>>
    {
    }

    public class GetNarratorOptionsQueryHandler : IQueryHandler<GetNarratorOptionsQuery, List<NarratorOptionDto>>
    {
        private readonly IUow _uow;

        public GetNarratorOptionsQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<List<NarratorOptionDto>>> Handle(GetNarratorOptionsQuery query, CancellationToken ct)
        {
            return await _uow.TrackRepository.GetNarratorOptionsAsync(ct);
        }
    }
}
