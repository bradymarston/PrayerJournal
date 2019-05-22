using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Filters
{
    public class FindUserFilter<TUser> : IAsyncActionFilter where TUser : IdentityUser
    {
        private readonly UserManager<TUser> _userManager;

        public FindUserFilter(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            if (actionContext.ActionArguments.ContainsKey("userId"))
            {
                var user = await _userManager.FindByIdAsync((string)actionContext.ActionArguments["userId"]);
                if (user is null)
                {
                    actionContext.Result = new NotFoundResult();
                    return;
                }

                actionContext.HttpContext.Items.Add("foundUser", user);
            }

            await next();
        }
    }
}
