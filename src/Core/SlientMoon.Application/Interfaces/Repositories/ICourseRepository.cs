using SlientMoon.Application.DTOs.Home;
using SlientMoon.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Interfaces.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {

        Task<List<HomeCourseRow>> GetRecommendedCoursesAsync(int limit, CancellationToken cancellationToken = default);


        Task<List<HomeCourseRow>> GetFeaturedCoursesByTypeAsync(string typeSlug, int limit, CancellationToken cancellationToken = default);

        Task<List<HomeCourseRow>> GetPopularCoursesByTypeAsync(string typeSlug, int limit, CancellationToken cancellationToken = default);
    }
}
