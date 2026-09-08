using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Admin;
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
    public class TrackRepository : GenericRepository<Track>, ITrackRepository
    {
        public TrackRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Track?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Narrator)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<AdminTrackListItemDto?> GetTrackByIdAsync(string trackId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(t => t.Id == trackId)
                .Select(ListItemProjection)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PagedResult<AdminTrackListItemDto>> GetPagedAsync(
            int page, int limit, string? search, string? courseId, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(courseId))
            {
                query = query.Where(t => t.CourseId == courseId);
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(ListItemProjection)
                .ToListAsync(cancellationToken);

            return new PagedResult<AdminTrackListItemDto>
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

        public async Task<List<NarratorOptionDto>> GetNarratorOptionsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Narrator>()
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .Select(n => new NarratorOptionDto
                {
                    Id = n.Id,
                    Name = n.Name,
                    Gender = n.Gender
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CourseOptionDto>> GetCourseOptionsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Course>()
                .AsNoTracking()
                .OrderBy(c => c.Title)
                .Select(c => new CourseOptionDto
                {
                    Id = c.Id,
                    Title = c.Title
                })
                .ToListAsync(cancellationToken);
        }

        private static readonly Expression<Func<Track, AdminTrackListItemDto>> ListItemProjection = t => new AdminTrackListItemDto
        {
            Id = t.Id,
            Title = t.Title,
            DurationSec = t.DurationSec,
            AudioUrl = t.AudioUrl,
            MimeType = t.MimeType,
            FileSizeBytes = t.FileSizeBytes,
            CourseId = t.CourseId,
            CourseTitle = t.Course != null ? t.Course.Title : null,
            NarratorId = t.NarratorId,
            NarratorName = t.Narrator != null ? t.Narrator.Name : null,
            CreatedAt = t.CreatedAt
        };
    }
}