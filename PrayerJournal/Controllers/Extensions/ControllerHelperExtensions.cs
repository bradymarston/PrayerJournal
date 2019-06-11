using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrayerJournal.Controllers.Extensions
{
    public static class ControllerHelperExtensions
    {
        public static async Task<TUser> GetAuthorizedUser<TUser>(this IControllerWithUserManager<TUser> controller)
            where TUser : IdentityUser
        {
            var userId = controller.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return null;

            return await controller.UserManager.FindByIdAsync(userId);
        }
    }
}
