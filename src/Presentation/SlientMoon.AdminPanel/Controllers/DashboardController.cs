using Microsoft.AspNetCore.Mvc;
using SlientMoon.AdminPanel.Models.Dashboard;
using SlientMoon.Application.Features.Admin.Courses.Queries.GetAdminCourses;
using SlientMoon.Application.Features.Admin.Tracks.Queries.GetAdminTracks;
using SlientMoon.Application.Features.Categories.Queries.GetAdminCategories;
using SlientMoon.Application.Features.CategoryTypes.Queries.GetCategoryTypes;

namespace SlientMoon.AdminPanel.Controllers
{
    public class DashboardController : BaseAdminController
    {
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var recentCoursesTask = Dispatcher.Send(new GetAdminCoursesQuery { Page = 1, Limit = 5 }, ct);
            var courseCountTask = Dispatcher.Send(new GetAdminCoursesQuery { Page = 1, Limit = 1 }, ct);
            var categoryCountTask = Dispatcher.Send(new GetAdminCategoriesQuery { Page = 1, Limit = 1 }, ct);
            var categoryTypeCountTask = Dispatcher.Send(new GetCategoryTypesQuery { Page = 1, Limit = 1 }, ct);
            var trackCountTask = Dispatcher.Send(new GetAdminTracksQuery { Page = 1, Limit = 1 }, ct);

            await Task.WhenAll(recentCoursesTask, courseCountTask, categoryCountTask, categoryTypeCountTask, trackCountTask);

            var recentCoursesResult = await recentCoursesTask;
            var courseCountResult = await courseCountTask;  
            var categoryCountResult = await categoryCountTask;
            var categoryTypeCountResult = await categoryTypeCountTask;
            var trackCountResult = await trackCountTask;

            var model = new DashboardViewModel
            {
                CourseCount = courseCountResult.IsSuccess ? courseCountResult.Value.Meta.Total : 0,
                CategoryCount = categoryCountResult.IsSuccess ? categoryCountResult.Value.Meta.Total : 0,
                CategoryTypeCount = categoryTypeCountResult.IsSuccess ? categoryTypeCountResult.Value.Meta.Total : 0,
                TrackCount = trackCountResult.IsSuccess ? trackCountResult.Value.Meta.Total : 0,
                RecentCourses = recentCoursesResult.IsSuccess ? recentCoursesResult.Value.Data : new()
            };

            return View(model);
        }
    }
}
