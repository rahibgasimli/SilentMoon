using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.DTOs.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface IAdminCourseRepository
    {
        Task<string> InsertCourseAsync(AdminCourseInput course, CancellationToken ct = default);

        Task UpdateCourseAsync(string courseId, AdminCourseInput course, CancellationToken ct = default);

        Task DeleteCourseAsync(string courseId, CancellationToken ct = default);

        Task<AdminCourseListItemDto> GetByIdAsync(string courseId, CancellationToken ct = default);

        Task<PagedResult<AdminCourseListItemDto>> GetPagedAsync(int page, int limit, string? search, CancellationToken ct = default);

        Task<List<CategoryOptionDto>> GetCategoryOptionsAsync(CancellationToken ct = default);

    }
}
