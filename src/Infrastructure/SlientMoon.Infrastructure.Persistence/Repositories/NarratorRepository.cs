using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.DTOs.Narrators;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Domain.Entities;
using SlientMoon.Infrastructure.Persistence.Contexts;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories
{
    public class NarratorRepository : GenericRepository<Narrator>, INarratorRepository
    {
        public NarratorRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PagedResult<NarratorListItemDto>> GetPagedAsync(
            int page, int limit, string? search, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(term));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(n => new NarratorListItemDto
                {
                    Id = n.Id,
                    Name = n.Name,
                    Gender = n.Gender,
                    TrackCount = _dbContext.Set<Track>().Count(t => t.NarratorId == n.Id),
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<NarratorListItemDto>
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

        public async Task<bool> HasTracksAsync(string narratorId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Track>().AnyAsync(t => t.NarratorId == narratorId, cancellationToken);
        }
    }
}
