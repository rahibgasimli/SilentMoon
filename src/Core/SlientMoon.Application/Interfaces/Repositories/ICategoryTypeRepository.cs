using SlientMoon.Application.DTOs.CategoryType;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface ICategoryTypeRepository : IGenericRepository<CategoryType>
    {
        Task<PagedResult<CategoryTypeListItemDto>> GetPagedAsync(
            int page, int limit, string? search, CancellationToken cancellationToken = default);
    }
}
