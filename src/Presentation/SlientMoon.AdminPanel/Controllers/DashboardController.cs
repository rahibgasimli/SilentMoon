using Microsoft.AspNetCore.Mvc;
using SlientMoon.Application.Features.Categories.Queries.GetCategories;

namespace SlientMoon.AdminPanel.Controllers
{
    public class DashboardController : BaseAdminController
    {
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await Dispatcher.Send(new GetCategoriesQuery(), ct);

            ViewBag.IsSuccess = result.IsSuccess;
            ViewBag.CategoryCount = result.IsSuccess ? result.Value.Count : 0;
            ViewBag.ErrorCode = result.IsSuccess ? null : result.Error?.Code;

            return View();
        }
    }
}
