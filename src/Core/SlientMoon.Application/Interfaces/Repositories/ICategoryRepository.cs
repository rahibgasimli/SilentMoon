using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<PagedResult<CategoryListItemDto>> GetPagedAsync(
            int page, int limit, string? search, string? categoryTypeId, CancellationToken cancellationToken = default);

        Task<CategoryListItemDto?> GetListItemByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<List<CategoryTypeOptionDto>> GetCategoryTypeOptionsAsync(CancellationToken cancellationToken = default);
    }
}
