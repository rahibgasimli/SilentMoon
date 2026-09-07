using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Categories;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.CategoryType)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<PagedResult<CategoryListItemDto>> GetPagedAsync(
           int page, int limit, string? search, string? categoryTypeId, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(categoryTypeId))
            {
                query = query.Where(c => c.CategoryTypeId == categoryTypeId);
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(ListItemProjection)
                .ToListAsync(cancellationToken);

            return new PagedResult<CategoryListItemDto>
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

        public async Task<CategoryListItemDto?> GetListItemByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(ListItemProjection)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<CategoryTypeOptionDto>> GetCategoryTypeOptionsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<CategoryType>()
                .AsNoTracking()
                .OrderBy(ct => ct.Name)
                .Select(ct => new CategoryTypeOptionDto
                {
                    Id = ct.Id,
                    Name = ct.Name
                })
                .ToListAsync(cancellationToken);
        }

        private static readonly Expression<Func<Category, CategoryListItemDto>> ListItemProjection = c => new CategoryListItemDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            IconUrl = c.IconUrl,
            CategoryTypeId = c.CategoryTypeId,
            CategoryTypeName = c.CategoryType != null ? c.CategoryType.Name : null,
            CreatedAt = c.CreatedAt
        };

    }
}
