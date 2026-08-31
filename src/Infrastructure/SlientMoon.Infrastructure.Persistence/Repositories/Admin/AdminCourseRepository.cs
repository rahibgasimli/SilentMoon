using Dapper;
using SlientMoon.Application.DTOs.Admin;
using SlientMoon.Application.DTOs.Common;
using SlientMoon.Application.Interfaces.Repositories;
using SlientMoon.Infrastructure.Persistence.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlientMoon.Infrastructure.Persistence.Repositories.Admin
{
    public class AdminCourseRepository : IAdminCourseRepository
    {
        private readonly IDapper _dapper;

        public AdminCourseRepository(IDapper dapper)
        {
            _dapper = dapper;
        }

        private const string SelectColumns = @"
            c.""Id""          AS ""Id"",
            c.""Title""       AS ""Title"",
            c.""Subtitle""    AS ""Subtitle"",
            c.""Description"" AS ""Description"",
            c.""ImageUrl""    AS ""ImageUrl"",
            c.""DurationSec"" AS ""DurationSec"",
            c.""IsFeatured""  AS ""IsFeatured"",
            c.""ViewCount""   AS ""ViewCount"",
            c.""CategoryId""  AS ""CategoryId"",
            cat.""Name""      AS ""CategoryName"",
            c.""CreatedAt""   AS ""CreatedAt""";

        private const string FromClause = @"
            FROM ""Courses"" c
            LEFT JOIN ""Categories"" cat ON cat.""Id"" = c.""CategoryId""";

        public async Task<string> InsertCourseAsync(AdminCourseInput course, CancellationToken ct = default)
        {
            var id = Guid.NewGuid().ToString();

            const string sql = @"
                INSERT INTO ""Courses""
                    (""Id"", ""Title"", ""Subtitle"", ""Description"", ""ImageUrl"",
                     ""DurationSec"", ""IsFeatured"", ""ViewCount"", ""CategoryId"", ""CreatedAt"")
                VALUES
                    (:P_ID, :P_TITLE, :P_SUBTITLE, :P_DESCRIPTION, :P_IMAGEURL,
                     :P_DURATIONSEC, :P_ISFEATURED, 0, :P_CATEGORYID, :P_CREATEDAT)";

            var parameters = BuildParameters(course);
            parameters.Add("P_ID", id);
            parameters.Add("P_CREATEDAT", DateTime.UtcNow);

            await _dapper.ExecuteAsync(sql, parameters);

            return id;
        }

        public async Task UpdateCourseAsync(string courseId, AdminCourseInput course, CancellationToken ct = default)
        {
            const string sql = @"
                UPDATE ""Courses"" SET
                    ""Title"" = :P_TITLE,
                    ""Subtitle"" = :P_SUBTITLE,
                    ""Description"" = :P_DESCRIPTION,
                    ""ImageUrl"" = :P_IMAGEURL,
                    ""DurationSec"" = :P_DURATIONSEC,
                    ""IsFeatured"" = :P_ISFEATURED,
                    ""CategoryId"" = :P_CATEGORYID
                WHERE ""Id"" = :P_ID";

            var parameters = BuildParameters(course);
            parameters.Add("P_ID", courseId);

            await _dapper.ExecuteAsync(sql, parameters);
        }

        public async Task DeleteCourseAsync(string courseId, CancellationToken ct = default)
        {
            const string sql = @"DELETE FROM ""Courses"" WHERE ""Id"" = :P_ID";

            var parameters = new DynamicParameters();
            parameters.Add("P_ID", courseId);

            await _dapper.ExecuteAsync(sql, parameters);
        }

        public async Task<AdminCourseListItemDto> GetByIdAsync(string courseId, CancellationToken ct = default)
        {
            var sql = $@"SELECT {SelectColumns} {FromClause} WHERE c.""Id"" = :P_ID";

            var parameters = new DynamicParameters();
            parameters.Add("P_ID", courseId);

            return await _dapper.GetAsync<AdminCourseListItemDto>(sql, parameters);
        }

        public async Task<PagedResult<AdminCourseListItemDto>> GetPagedAsync(
            int page, int limit, string? search, CancellationToken ct = default)
        {
            var hasSearch = !string.IsNullOrWhiteSpace(search);
            var offset = (page - 1) * limit;

            var whereClause = hasSearch
                ? @"WHERE LOWER(c.""Title"") LIKE LOWER(:P_SEARCH) ESCAPE '\'"
                : string.Empty;

            var countSql = $@"SELECT COUNT(*) {FromClause} {whereClause}";

            var countParameters = new DynamicParameters();
            if (hasSearch)
                countParameters.Add("P_SEARCH", "%" + EscapeLike(search!.Trim()) + "%");

            var total = await _dapper.ExecuteScalarAsync<int>(countSql, countParameters);

            var pageSql = $@"
                SELECT {SelectColumns}
                {FromClause}
                {whereClause}
                ORDER BY c.""CreatedAt"" DESC
                OFFSET :P_OFFSET ROWS FETCH NEXT :P_LIMIT ROWS ONLY";

            var pageParameters = new DynamicParameters();
            if (hasSearch)
                pageParameters.Add("P_SEARCH", "%" + EscapeLike(search!.Trim()) + "%");
            pageParameters.Add("P_OFFSET", offset);
            pageParameters.Add("P_LIMIT", limit);

            var items = (await _dapper.GetAllAsync<AdminCourseListItemDto>(pageSql, pageParameters)).ToList();

            return new PagedResult<AdminCourseListItemDto>
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

        public async Task<List<CategoryOptionDto>> GetCategoryOptionsAsync(CancellationToken ct = default)
        {
            const string sql = @"
                SELECT ""Id"" AS ""Id"", ""Name"" AS ""Name""
                FROM ""Categories""
                ORDER BY ""Name""";

            var result = await _dapper.GetAllAsync<CategoryOptionDto>(sql);
            return result.ToList();
        }

        private static DynamicParameters BuildParameters(AdminCourseInput course)
        {
            var parameters = new DynamicParameters();
            parameters.Add("P_TITLE", course.En.Title);
            parameters.Add("P_SUBTITLE", course.En.Subtitle);
            parameters.Add("P_DESCRIPTION", course.En.Description);
            parameters.Add("P_IMAGEURL", course.ImageUrl);
            parameters.Add("P_DURATIONSEC", course.DurationSec);
            parameters.Add("P_ISFEATURED", course.IsFeatured ? 1 : 0);
            parameters.Add("P_CATEGORYID", course.CategoryId);
            return parameters;
        }

        private static string EscapeLike(string value) =>
            value.Replace(@"\", @"\\").Replace("_", @"\_").Replace("%", @"\%");
    }
}