using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrayerJournal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Identity
{
    public class UserPendingEmailStore<TUser, TDbContext> : UserStore<TUser>, IUserPendingEmailStore<TUser>
        where TUser : IdentityUser, IPendingEmailUser, new()
        where TDbContext : DbContext
    {
        public UserPendingEmailStore(TDbContext context, IdentityErrorDescriber describer = null) 
            : base(context, describer)
        {
        }

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized pending email address.
        /// </summary>
        /// <param name="normalizedPendingEmail">The normalized pending email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public Task<TUser> FindByPendingEmailAsync(string normalizedPendingEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Task.FromResult(Users.Where(u => u.NormalizedPendingEmail == normalizedPendingEmail).ToList().SingleOrDefault());
        }

        /// <summary>
        /// Returns the normalized pending email for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email address to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the normalized email address if any associated with the specified user.
        /// </returns>
        public Task<string> GetNormalizedPendingEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.NormalizedPendingEmail);
        }

        /// <summary>
        /// Gets the pending email address for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object containing the results of the asynchronous operation, the email address for the specified <paramref name="user"/>.</returns>
        public Task<string> GetPendingEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PendingEmail);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user"/> has a pending email address change.
        /// </summary>
        /// <param name="user">The user whose pending email status should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, a flag indicating whether the specified <paramref name="user"/> has a pending
        /// email address.
        /// </returns>
        public Task<bool> HasPendingEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PendingEmail != null);
        }

        /// <summary>
        /// Sets the normalized pending email for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email address to set.</param>
        /// <param name="normalizedPendingEmail">The normalized pending email to set for the specified <paramref name="user"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task SetNormalizedPendingEmailAsync(TUser user, string normalizedPendingEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedPendingEmail = normalizedPendingEmail;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the <paramref name="pendingEmail"/> address for a <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email should be set.</param>
        /// <param name="pendingEmail">The pending email to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task SetPendingEmailAsync(TUser user, string pendingEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PendingEmail = pendingEmail;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears the pending email address for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose pending email address should be cleared.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public Task ClearPendingEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PendingEmail = null;
            user.NormalizedPendingEmail = null;
            return Task.CompletedTask;
        }
    }
}
