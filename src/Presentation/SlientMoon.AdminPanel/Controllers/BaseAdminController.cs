using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlientMoon.SharedKernel.Primitives;

namespace SlientMoon.AdminPanel.Controllers
{
    [Authorize]
    public abstract class BaseAdminController : Controller
    {
        protected IDispatcher Dispatcher =>
            HttpContext.RequestServices.GetRequiredService<IDispatcher>();

        protected void AddResultErrorsToModelState(Error error)
        {
            if (error is ValidationError validationError)
            {
                foreach (var validationFailure in validationError.Errors)
                    ModelState.AddModelError(string.Empty, validationFailure.Description);

                return;
            }

            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
