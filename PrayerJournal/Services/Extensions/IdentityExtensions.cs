using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Services.Extensions
{
    public static class IdentityExtensions
    {
        public static void EnsureRolesExist<TRole>(this RoleManager<TRole> roleManager, IEnumerable<TRole> roles)
            where TRole : IdentityRole
        {
            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role.Name).Result)
                    roleManager.CreateAsync(role).Wait();
            }
        }

        public static void EnsureRoleUserExists<TUser>(this UserManager<TUser> userManager, string roleName, TUser defaultUser, string defaultPassword)
            where TUser : IdentityUser
        {
            if (userManager.GetUsersInRoleAsync(roleName).Result.Count == 0)
            {
                var result = userManager.CreateAsync(defaultUser, defaultPassword).Result;
                if (result.Succeeded)
                {
                    userManager.ConfirmEmailAsync(defaultUser, userManager.GenerateEmailConfirmationTokenAsync(defaultUser).Result).Wait();
                    userManager.AddToRoleAsync(defaultUser, roleName).Wait();
                }
            }
        }
    }
}
