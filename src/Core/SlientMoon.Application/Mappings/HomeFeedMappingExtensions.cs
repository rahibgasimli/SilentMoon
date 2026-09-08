using SlientMoon.Application.Common.Extensions;
using SlientMoon.Application.DTOs.Courses;
using SlientMoon.Application.DTOs.Home;
using SlientMoon.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SlientMoon.Application.Mappings
{
    public static class HomeFeedMappingExtensions
    {
        public static CourseDto ToHomeCourseDto(
            this Course course,
            Dictionary<string, string>? translations = null,
            int? trackCount = null)
        {
            return new CourseDto
            {
                Id = course.Id,
                Title = course.GetTranslation(c => c.Title, translations),
                Subtitle = course.GetTranslation(c => c.Subtitle, translations),
                Description = course.GetTranslation(c => c.Description, translations),
                Type = course.Category?.CategoryType?.Slug?.ToLower() ?? string.Empty,
                ImageUrl = course.ImageUrl,
                DurationSec = course.DurationSec,
                IsFeatured = course.IsFeatured,
                CreatedAt = course.CreatedAt,
                CategoryId = course.CategoryId,
                TrackCount = trackCount ?? (course.Tracks != null ? course.Tracks.Count : 0),
                Narrators = course.CourseNarrators != null
                    ? course.CourseNarrators
                        .Where(cn => cn.Narrator != null)
                        .Select(cn => cn.Narrator.Gender.ToString().ToLower())
                        .ToList() : new()
            };
        }

        /// <summary>
        /// DB-dən projeksiya ilə gələn sətri DTO-ya çevirir.
        /// Tərcümə açarları Course entity-si əsasında qurulur.
        /// </summary>
        public static CourseDto ToHomeCourseDto(
            this HomeCourseRow row,
            Dictionary<string, string>? translations = null)
        {
            return new CourseDto
            {
                Id = row.Id,
                Title = ResolveTranslation(nameof(Course.Title), row.Id, row.Title, translations),
                Subtitle = ResolveTranslation(nameof(Course.Subtitle), row.Id, row.Subtitle, translations),
                Description = ResolveTranslation(nameof(Course.Description), row.Id, row.Description, translations),
                Type = row.Type ?? string.Empty,
                ImageUrl = row.ImageUrl,
                DurationSec = row.DurationSec,
                IsFeatured = row.IsFeatured,
                CreatedAt = row.CreatedAt,
                CategoryId = row.CategoryId,
                TrackCount = row.TrackCount,
                Narrators = row.NarratorGenders
                    .Select(g => g.ToString().ToLower())
                    .Distinct()
                    .ToList()
            };
        }

        private static string ResolveTranslation(
            string propertyName,
            string entityId,
            string? fallback,
            Dictionary<string, string>? translations)
        {
            if (translations is not null)
            {
                var key = TranslationExtensions.BuildKey<Course>(propertyName, entityId);

                if (translations.TryGetValue(key, out var translated))
                    return translated;
            }

            return fallback ?? string.Empty;
        }

        public static DailyThoughtDto ToDailyThoughtDto(this DailyThought dailyThought)
        {
            return new DailyThoughtDto
            {
                Id = dailyThought.Id,
                Title = dailyThought.Course.Title,
                Subtitle = dailyThought.Course.Subtitle,
                DurationSec = dailyThought.Course.DurationSec,
                CourseId = dailyThought.CourseId
            };
        }
    }
}