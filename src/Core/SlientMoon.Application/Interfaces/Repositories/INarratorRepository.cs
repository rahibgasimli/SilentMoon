using SlientMoon.Application.DTOs.Narrators;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface INarratorRepository : IGenericRepository<Narrator>
    {
        Task<PagedResult<NarratorListItemDto>> GetPagedAsync(
            int page, int limit, string? search, CancellationToken cancellationToken = default);

        Task<bool> HasTracksAsync(string narratorId, CancellationToken cancellationToken = default);
    }
}
