using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace PrayerJournal.Controllers.Extensions
{
    public static class ControllerHelperExtensions
    {
        public static string GetCurrentUserName(this HttpContext context)
        {
            return ((ClaimsIdentity)context.User.Identity).Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}