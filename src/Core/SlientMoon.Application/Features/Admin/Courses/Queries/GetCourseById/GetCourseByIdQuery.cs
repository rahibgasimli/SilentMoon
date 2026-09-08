using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Domain.Constants;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.Domain.Errors;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Admin.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQuery : IQuery<AdminCourseInput>
    {
        public string Id { get; set; }
    }

    public class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, AdminCourseInput>
    {
        private readonly IAdminCourseRepository _adminCourseRepository;
        private readonly ITranslationService _translationService;

        public GetCourseByIdQueryHandler(
            IAdminCourseRepository adminCourseRepository,
            ITranslationService translationService)
        {
            _adminCourseRepository = adminCourseRepository;
            _translationService = translationService;
        }

        public async Task<Result<AdminCourseInput>> Handle(GetCourseByIdQuery query, CancellationToken ct)
        {
            var course = await _adminCourseRepository.GetByIdAsync(query.Id, ct);
            if (course is null)
                return CourseErrors.NotFound;

            var languages = await _translationService.GetTranslationMapAsync<Course>(
                query.Id, TranslatableFields.Course, ct);

            languages.Set(LanguageCode.EN, TranslatableField.Title, course.Title);
            languages.Set(LanguageCode.EN, TranslatableField.Subtitle, course.Subtitle);
            languages.Set(LanguageCode.EN, TranslatableField.Description, course.Description);

            return new AdminCourseInput
            {
                ImageUrl = course.ImageUrl,
                DurationSec = course.DurationSec,
                IsFeatured = course.IsFeatured,
                CategoryId = course.CategoryId,
                NarratorIds = course.NarratorIds,
                Languages = languages
            };
        }
    }
}
