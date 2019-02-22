using Microsoft.AspNetCore.Mvc;

namespace PrayerJournal.Controllers.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult BadModel(this ControllerBase controller)
        {
            return controller.BadRequest(new ValidationProblemDetails(controller.ModelState));
        }
        public static IActionResult BadModel(this ControllerBase controller, string key, string errorMessage)
        {
            controller.ModelState.AddModelError(key, errorMessage);
            return controller.BadModel();
        }
    }
}
