using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SlientMoon.AdminPanel.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public abstract class BaseAdminController : Controller
    {
        protected IDispatcher Dispatcher =>
            HttpContext.RequestServices.GetRequiredService<IDispatcher>();
    }
}
