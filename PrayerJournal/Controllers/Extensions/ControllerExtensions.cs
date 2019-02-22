using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

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

        public static string GetCurrentUserName(this HttpContext context)
        {
            return ((ClaimsIdentity)context.User.Identity).Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
