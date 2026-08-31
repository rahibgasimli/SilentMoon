using Application.Abstractions.Messaging;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Application.Interfaces.Services;
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

            var fields = new[] { TranslatableField.Title, TranslatableField.Subtitle, TranslatableField.Description };
            var keys = new List<string>();
            foreach (var field in fields)
                keys.Add(TranslationExtensions.BuildKey<Course>(field.ToString(), query.Id));

            var azTranslations = await _translationService.GetTranslationsAsync(keys, LanguageCode.AZ, ct);
            var ruTranslations = await _translationService.GetTranslationsAsync(keys, LanguageCode.RU, ct);

            string Get(Dictionary<string, string> dict, TranslatableField field) =>
                    dict.TryGetValue(TranslationExtensions.BuildKey<Course>(field.ToString(), query.Id), out var value)
                    ? value
                    : null;

            return new AdminCourseInput
            {
                ImageUrl = course.ImageUrl,
                DurationSec = course.DurationSec,
                IsFeatured = course.IsFeatured,
                CategoryId = course.CategoryId,
                En = new AdminCourseTranslationInput
                {
                    Title = course.Title,
                    Subtitle = course.Subtitle,
                    Description = course.Description,
                },
                Az = new AdminCourseTranslationInput
                {
                    Title = Get(azTranslations, TranslatableField.Title),
                    Subtitle = Get(azTranslations, TranslatableField.Subtitle),
                    Description = Get(azTranslations, TranslatableField.Description),
                },
                Ru = new AdminCourseTranslationInput
                {
                    Title = Get(ruTranslations, TranslatableField.Title),
                    Subtitle = Get(ruTranslations, TranslatableField.Subtitle),
                    Description = Get(ruTranslations, TranslatableField.Description)
                }
            };
        }
    }
}
