using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.CategoryType;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class CategoryTypeRepository : GenericRepository<CategoryType>, ICategoryTypeRepository
    {
        public CategoryTypeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PagedResult<CategoryTypeListItemDto>> GetPagedAsync(
            int page, int limit, string? search, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(ct => ct.Name.ToLower().Contains(term));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(ct => ct.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(ListItemProjection)
                .ToListAsync(cancellationToken);

            return new PagedResult<CategoryTypeListItemDto>
            {
                Data = items,
                Meta = new PageMeta
                {
                    Page = page,
                    Limit = limit,
                    Total = total,
                    TotalPages = (int)Math.Ceiling((double)total / limit)
                }
            };
        }

        public async Task<CategoryTypeListItemDto?> GetListItemByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(ct => ct.Id == id)
                .Select(ListItemProjection)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private static readonly Expression<Func<CategoryType, CategoryTypeListItemDto>> ListItemProjection = ct => new CategoryTypeListItemDto
        {
            Id = ct.Id,
            Name = ct.Name,
            Slug = ct.Slug,
            CategoryCount = ct.Categories.Count,
            CreatedAt = ct.CreatedAt
        };
    }
}
