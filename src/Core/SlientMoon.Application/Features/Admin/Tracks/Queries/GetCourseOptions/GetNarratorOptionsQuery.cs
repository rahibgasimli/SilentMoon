using Application.Abstractions.Messaging;
using SlientMoon.Application.DTOs.Admin;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Tracks.Queries.GetCourseOptions
{
    public class GetCourseOptionsQuery : IQuery<List<CourseOptionDto>>
    {
    }

    public class GetCourseOptionsQueryHandler : IQueryHandler<GetCourseOptionsQuery, List<CourseOptionDto>>
    {
        private readonly IUow _uow;

        public GetCourseOptionsQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<List<CourseOptionDto>>> Handle(GetCourseOptionsQuery query, CancellationToken ct)
        {
            return await _uow.TrackRepository.GetCourseOptionsAsync(ct);
        }
    }
}
