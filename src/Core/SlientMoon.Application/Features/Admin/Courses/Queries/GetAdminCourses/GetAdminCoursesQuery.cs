using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.Interfaces.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Courses.Queries.GetAdminCourses
{
    public class GetAdminCoursesQuery : IQuery<PagedResult<AdminCourseListItemDto>>
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? Search { get; set; }
    }

    public class GetAdminCoursesQueryHandler : IQueryHandler<GetAdminCoursesQuery, PagedResult<AdminCourseListItemDto>>
    {
        private readonly IAdminCourseRepository _adminCourseRepository;

        public GetAdminCoursesQueryHandler(IAdminCourseRepository adminCourseRepository)
        {
            _adminCourseRepository = adminCourseRepository;
        }

        public async Task<Result<PagedResult<AdminCourseListItemDto>>> Handle(GetAdminCoursesQuery query, CancellationToken ct)
        {
            var result = await _adminCourseRepository.GetPagedAsync(query.Page, query.Limit, query.Search, ct);
            return result;
        }
    }
}
