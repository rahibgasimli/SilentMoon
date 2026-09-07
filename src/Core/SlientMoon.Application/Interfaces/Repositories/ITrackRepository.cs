using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface ITrackRepository : IGenericRepository<Track>
    {
        Task<AdminTrackListItemDto?> GetTrackByIdAsync(string trackId, CancellationToken cancellationToken = default);

        Task<PagedResult<AdminTrackListItemDto>> GetPagedAsync(
            int page, int limit, string? search, string? courseId, CancellationToken cancellationToken = default);

        Task<List<NarratorOptionDto>> GetNarratorOptionsAsync(CancellationToken cancellationToken = default);

        Task<List<CourseOptionDto>> GetCourseOptionsAsync(CancellationToken cancellationToken = default);
    }
}
