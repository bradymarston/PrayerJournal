using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Authentication
{
    public static class ShadyAuthenticationExtensions
    {
        public static AuthenticationBuilder AddShady<TUser>(this AuthenticationBuilder builder)
            where TUser : IdentityUser, IShadyUser
        {
            return AddShady<TUser>(builder, ShadyAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddShady<TUser>(this AuthenticationBuilder builder, string authenticationScheme)
            where TUser : IdentityUser, IShadyUser
        {
            return AddShady<TUser>(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddShady<TUser>(this AuthenticationBuilder builder, Action<ShadyAuthenticationOptions> configureOptions)
            where TUser : IdentityUser, IShadyUser
        {
            return AddShady<TUser>(builder, ShadyAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddShady<TUser>(this AuthenticationBuilder builder, string authenticationScheme, Action<ShadyAuthenticationOptions> configureOptions)
            where TUser : IdentityUser, IShadyUser
        {
            builder.Services.AddSingleton<IPostConfigureOptions<ShadyAuthenticationOptions>, ShadyAuthenticationPostConfigureOptions>();
            builder.Services.AddScoped<IShadyTokenService, ShadyTokenService>();
            builder.Services.AddScoped<IShadySignInManager<TUser>, ShadySignInManager<TUser>>();

            builder.Services.AddDataProtection();

            return builder.AddScheme<ShadyAuthenticationOptions, ShadyAuthenticationHandler<TUser>>(
                authenticationScheme, configureOptions);
        }

        public static TUser GetAuthenticatedUser<TUser>(this Controller controller)
            where TUser : IdentityUser, IShadyUser
        {
            return (TUser)controller.HttpContext.Items["AuthenticatedUser"];
        }
    }
}
