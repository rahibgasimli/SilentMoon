using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Courses.Queries.GetCategoryOptions
{
    public class GetCategoryOptionsQuery : IQuery<List<CategoryOptionDto>>
    {
    }

    public class GetCategoryOptionsQueryHandler : IQueryHandler<GetCategoryOptionsQuery, List<CategoryOptionDto>>
    {
        private readonly IAdminCourseRepository _adminCourseRepository;

        public GetCategoryOptionsQueryHandler(IAdminCourseRepository adminCourseRepository)
        {
            _adminCourseRepository = adminCourseRepository;
        }

        public async Task<Result<List<CategoryOptionDto>>> Handle(GetCategoryOptionsQuery query, CancellationToken ct)
        {
            var options = await _adminCourseRepository.GetCategoryOptionsAsync(ct);
            return options;
        }
    }
}
