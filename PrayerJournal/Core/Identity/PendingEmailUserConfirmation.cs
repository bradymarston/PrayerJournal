using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Identity
{
    public class PendingEmailUserConfirmation<TUser> : IUserConfirmation<TUser> where TUser : class
    {
        /// <summary>
        /// Determines whether the specified <paramref name="user"/> is confirmed.
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> that can be used to retrieve user properties.</param>
        /// <param name="user">The user.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the confirmation operation.</returns>
        public async virtual Task<bool> IsConfirmedAsync(UserManager<TUser> manager, TUser user)
        {
            if (!await manager.IsEmailConfirmedAsync(user) && (await manager.GetLoginsAsync(user)).Count == 0)
            {
                return false;
            }
            return true;
        }
    }
}