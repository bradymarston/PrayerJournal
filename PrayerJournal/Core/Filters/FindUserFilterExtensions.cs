using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace PrayerJournal.Core.Filters
{
    public static class FindUserFilterExtensions
    {
        public static TUser GetFoundUser<TUser>(this HttpContext context) where TUser : IdentityUser
        {
            var user = (TUser)context.Items["foundUser"];

            if (user == null)
                throw new Exception("User not found in GetFoundUser. Did you fail to call the filter?");

            return user;
        }

        public static IServiceCollection AddFindUserFilter<TUser>(this IServiceCollection services)
            where TUser : IdentityUser
        {
            return services.AddScoped<FindUserFilter<TUser>>();
        }
    }
}