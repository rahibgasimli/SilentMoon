using Application.Abstractions.Messaging;
using Microsoft.Extensions.Localization;
using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Home;
using SlientMoon.Application.Interfaces.Logging;
using SlientMoon.Application.Interfaces.Services;
using SlientMoon.Application.Mappings;
using SlientMoon.Domain.Entities;
using SlientMoon.Domain.Enums;
using SlientMoon.SharedKernel.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Application.Features.Home.Queries.GetHomeFeed
{
    public class GetHomeFeedQuery : IQuery<HomeDto>
    {
        public string? Language { get; set; }

        public GetHomeFeedQuery(string? language = null)
        {
            Language = language;
        }
    }

    public class GetHomeFeedQueryHandler : IQueryHandler<GetHomeFeedQuery, HomeDto>
    {
        private const int HomeFeedBlockSize = 4;
        private const string SleepTypeSlug = "sleep";
        private const string MeditationTypeSlug = "meditation";

        private readonly IUow _uow;
        private readonly IAppLogger<GetHomeFeedQueryHandler> _logger;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILanguageService _languageService;
        private readonly ITranslationService _translationService;

        public GetHomeFeedQueryHandler(
            IUow uow,
            IAppLogger<GetHomeFeedQueryHandler> logger,
            IDateTimeService dateTimeService,
            ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer,
            ILanguageService languageService,
            ITranslationService translationService)
        {
            _uow = uow;
            _logger = logger;
            _dateTimeService = dateTimeService;
            _currentUserService = currentUserService;
            _localizer = localizer;
            _languageService = languageService;
            _translationService = translationService;
        }

        public async Task<Result<HomeDto>> Handle(GetHomeFeedQuery query, CancellationToken ct)
        {
            string userId = _currentUserService.GetUser();
            LanguageCode currentLang = _languageService.ValidateLanguage(query.Language);

            _logger.LogInformation("Home feed məlumatları sorğulanır.");

            var user = await _uow.UserRepository.GetByIdAsync(userId, ct);
            string userName = user?.Name ?? "User";
            var now = _dateTimeService.NowUtc;
            var hour = now.Hour;

            string greetingKey = hour switch
            {
                >= 5 and < 12 => "Greeting.Morning",
                >= 12 and < 18 => "Greeting.Afternoon",
                _ => "Greeting.Evening"
            };

            var greetingDto = new GreetingDto
            {
                Title = $"{_localizer[greetingKey]}, {userName}",
                Message = _localizer["Greeting.Submessage"].Value
            };
            var recommendedTitle = _localizer["Home.RecommendedForYou"].Value;

            var today = _dateTimeService.NowUtc.Date;

            var dailyThoughtEntity = await _uow.DailyThoughtRepository.GetDailyThoughtByDateAsync(today, ct);

            DailyThoughtDto? dailyThoughtDto = null;
            if (dailyThoughtEntity?.Course != null)
            {
                dailyThoughtDto = dailyThoughtEntity.ToDailyThoughtDto();
            }

            var recommendedRows = await _uow.CourseRepository
                .GetRecommendedCoursesAsync(HomeFeedBlockSize, ct);

            var featuredSleepRows = await _uow.CourseRepository
                .GetFeaturedCoursesByTypeAsync(SleepTypeSlug, HomeFeedBlockSize, ct);

            var popularMeditationRows = await _uow.CourseRepository
                .GetPopularCoursesByTypeAsync(MeditationTypeSlug, HomeFeedBlockSize, ct);

            Dictionary<string, string>? translations = null;
            if (currentLang != LanguageCode.EN)
            {
                var courseIds = recommendedRows
                    .Concat(featuredSleepRows)
                    .Concat(popularMeditationRows)
                    .Select(r => r.Id)
                    .Distinct()
                    .ToList();

                if (courseIds.Count > 0)
                {
                    var keys = courseIds.SelectMany(id => new[]
                    {
                        TranslationExtensions.BuildKey<Course>(nameof(Course.Title), id),
                        TranslationExtensions.BuildKey<Course>(nameof(Course.Subtitle), id),
                        TranslationExtensions.BuildKey<Course>(nameof(Course.Description), id)
                    });

                    translations = await _translationService.GetTranslationsAsync(keys, currentLang, ct);
                }
            }

            var response = new HomeDto
            {
                Greeting = greetingDto,
                RecommendedTitle = recommendedTitle,
                Recommended = recommendedRows.Select(r => r.ToHomeCourseDto(translations)).ToList(),
                DailyThought = dailyThoughtDto,
                FeaturedSleep = featuredSleepRows.Select(r => r.ToHomeCourseDto(translations)).ToList(),
                PopularMeditations = popularMeditationRows.Select(r => r.ToHomeCourseDto(translations)).ToList()
            };

            return response;
        }
    }
}