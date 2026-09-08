using Microsoft.EntityFrameworkCore;
using SlientMoon.Application.DTOs.Home;
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
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Course?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Category)
                    .ThenInclude(cat => cat.CategoryType)
                .Include(c => c.CourseNarrators)
                    .ThenInclude(cn => cn.Narrator)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<List<HomeCourseRow>> GetRecommendedCoursesAsync(
            int limit, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .Take(limit)
                .Select(HomeRowProjection)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<HomeCourseRow>> GetFeaturedCoursesByTypeAsync(
            string typeSlug, int limit, CancellationToken cancellationToken = default)
        {
            var normalizedSlug = typeSlug.ToLower();

            return await _dbSet
                .AsNoTracking()
                .Where(c => c.IsFeatured &&
                            c.Category.CategoryType.Slug.ToLower() == normalizedSlug)
                .OrderByDescending(c => c.CreatedAt)
                .Take(limit)
                .Select(HomeRowProjection)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<HomeCourseRow>> GetPopularCoursesByTypeAsync(
            string typeSlug, int limit, CancellationToken cancellationToken = default)
        {
            var normalizedSlug = typeSlug.ToLower();

            return await _dbSet
                .AsNoTracking()
                .Where(c => c.Category.CategoryType.Slug.ToLower() == normalizedSlug)
                .OrderByDescending(c => c.ViewCount)
                .Take(limit)
                .Select(HomeRowProjection)
                .ToListAsync(cancellationToken);
        }

        private static readonly Expression<Func<Course, HomeCourseRow>> HomeRowProjection = c => new HomeCourseRow
        {
            Id = c.Id,
            Title = c.Title,
            Subtitle = c.Subtitle,
            Description = c.Description,
            Type = c.Category.CategoryType.Slug.ToLower(),
            CategoryId = c.CategoryId,
            ImageUrl = c.ImageUrl,
            DurationSec = c.DurationSec,
            IsFeatured = c.IsFeatured,
            CreatedAt = c.CreatedAt,
            TrackCount = c.Tracks.Count,
            NarratorGenders = c.CourseNarrators
                .Where(cn => cn.Narrator != null)
                .Select(cn => cn.Narrator.Gender)
                .ToList()
        };
    }
}
