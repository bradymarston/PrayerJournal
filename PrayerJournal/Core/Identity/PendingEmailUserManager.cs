using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PrayerJournal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Identity
{
    public class PendingEmailUserManager<TUser> : UserManager<TUser>
        where TUser : IdentityUser, IPendingEmailUser, new()
    {
        private readonly IServiceProvider _services;
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public PendingEmailUserManager(IUserStore<TUser> store,
                                       IOptions<IdentityOptions> optionsAccessor,
                                       IPasswordHasher<TUser> passwordHasher,
                                       IEnumerable<IUserValidator<TUser>> userValidators,
                                       IEnumerable<IPasswordValidator<TUser>> passwordValidators,
                                       ILookupNormalizer keyNormalizer,
                                       IdentityErrorDescriber errors,
                                       IServiceProvider services,
                                       ILogger<UserManager<TUser>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _services = services;
        }

        /// <summary>
        /// Gets the pending email address for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email should be returned.</param>
        /// <returns>The task object containing the results of the asynchronous operation, the email address for the specified <paramref name="user"/>.</returns>
        public virtual async Task<string> GetPendingEmailAsync(TUser user)
        {
            ThrowIfDisposed();
            var store = GetPendingEmailStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return await store.GetPendingEmailAsync(user, CancellationToken);
        }

        /// <summary>
        /// Sets the <paramref name="pendingEmail"/> address for a <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose pending email should be set.</param>
        /// <param name="pendingEmail">The pending email to set.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public virtual async Task<IdentityResult> SetPendingEmailAsync(TUser user, string pendingEmail)
        {
            ThrowIfDisposed();
            var store = GetPendingEmailStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await store.SetPendingEmailAsync(user, pendingEmail, CancellationToken);
            return await UpdateUserAsync(user);
        }

        /// <summary>
        /// Clears any pending email address address for a <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose pending email should be cleared.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public virtual async Task<IdentityResult> ClearPendingEmailAsync(TUser user)
        {
            ThrowIfDisposed();
            var store = GetPendingEmailStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await store.ClearPendingEmailAsync(user, CancellationToken);
            return await UpdateUserAsync(user);
        }

        /// <summary>
        /// Gets the user, if any, associated with the normalized value of the specified pending email address.
        /// </summary>
        /// <param name="pendingEmail">The pending email address to return the user for.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user, if any, associated with a normalized value of the specified pending email address.
        /// </returns>
        public virtual async Task<TUser> FindByPendingEmailAsync(string pendingEmail)
        {
            ThrowIfDisposed();
            var store = GetPendingEmailStore();
            if (pendingEmail == null)
            {
                throw new ArgumentNullException(nameof(pendingEmail));
            }

            pendingEmail = NormalizeEmail(pendingEmail);
            var user = await store.FindByPendingEmailAsync(pendingEmail, CancellationToken);

            // Need to potentially check all keys
            if (user == null && Options.Stores.ProtectPersonalData)
            {
                var keyRing = _services.GetService<ILookupProtectorKeyRing>();
                var protector = _services.GetService<ILookupProtector>();
                if (keyRing != null && protector != null)
                {
                    foreach (var key in keyRing.GetAllKeyIds())
                    {
                        var oldKey = protector.Protect(key, pendingEmail);
                        user = await store.FindByPendingEmailAsync(oldKey, CancellationToken);
                        if (user != null)
                        {
                            return user;
                        }
                    }
                }
            }
            return user;
        }

        /// <summary>
        /// Updates the normalized pending email for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email address should be normalized and updated.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual async Task UpdateNormalizedPendingEmailAsync(TUser user)
        {
            var store = GetPendingEmailStore(throwOnFail: false);
            if (store != null)
            {
                var pendingEmail = await GetPendingEmailAsync(user);
                await store.SetNormalizedPendingEmailAsync(user, ProtectPersonalData(NormalizeEmail(pendingEmail)), CancellationToken);
            }
        }

        /// <summary>
        /// Validates that an email confirmation token matches the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to validate the token against.</param>
        /// <param name="token">The email confirmation token to validate.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        override public async Task<IdentityResult> ConfirmEmailAsync(TUser user, string token)
        {
            ThrowIfDisposed();
            var pendingEmailStore = GetPendingEmailStore();
            var emailStore = GetEmailStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var tokenResponse = await VerifyEmailConfirmationToken(user, token);
            if (!tokenResponse.Succeeded)
            {
                return tokenResponse;
            }

            var email = await pendingEmailStore.GetPendingEmailAsync(user, CancellationToken);
            await pendingEmailStore.ClearPendingEmailAsync(user, CancellationToken);

            await emailStore.SetEmailAsync(user, email, CancellationToken);
            await emailStore.SetEmailConfirmedAsync(user, true, CancellationToken);

            await UpdateSecurityStampInternal(user);

            return await UpdateUserAsync(user);
        }

        public async Task<IdentityResult> VerifyEmailConfirmationToken(TUser user, string token)
        {
            if (!await VerifyUserTokenAsync(user, Options.Tokens.EmailConfirmationTokenProvider, ConfirmEmailTokenPurpose, token))
            {
                return IdentityResult.Failed(ErrorDescriber.InvalidToken());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user"/> has a pending email address.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be returned.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
        /// has been confirmed or not.
        /// </returns>
        public virtual async Task<bool> HasPendingEmailAsync(TUser user)
        {
            ThrowIfDisposed();
            var store = GetPendingEmailStore();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return await store.HasPendingEmailAsync(user, CancellationToken);
        }

        protected override async Task<IdentityResult> UpdateUserAsync(TUser user)
        {
            await UpdateNormalizedPendingEmailAsync(user);
            return await base.UpdateUserAsync(user);
        }

        private IUserPendingEmailStore<TUser> GetPendingEmailStore(bool throwOnFail = true)
        {
            var cast = Store as IUserPendingEmailStore<TUser>;
            if (throwOnFail && cast == null)
            {
                throw new NotSupportedException("Store does not support pending email addresses.");
            }
            return cast;
        }

        private IUserEmailStore<TUser> GetEmailStore(bool throwOnFail = true)
        {
            var cast = Store as IUserEmailStore<TUser>;
            if (throwOnFail && cast == null)
            {
                throw new NotSupportedException("Store does not support email addresses.");
            }
            return cast;
        }

        private IUserSecurityStampStore<TUser> GetSecurityStore()
        {
            var cast = Store as IUserSecurityStampStore<TUser>;
            if (cast == null)
            {
                throw new NotSupportedException("Store does not support pending email store.");
            }
            return cast;
        }

        private string ProtectPersonalData(string data)
        {
            if (Options.Stores.ProtectPersonalData)
            {
                var keyRing = _services.GetService<ILookupProtectorKeyRing>();
                var protector = _services.GetService<ILookupProtector>();
                return protector.Protect(keyRing.CurrentKeyId, data);
            }
            return data;
        }
        private static string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
            _rng.GetBytes(bytes);
            return Encoding.UTF8.GetString(bytes);
        }

        // Update the security stamp if the store supports it
        private async Task UpdateSecurityStampInternal(TUser user)
        {
            if (SupportsUserSecurityStamp)
            {
                await GetSecurityStore().SetSecurityStampAsync(user, NewSecurityStamp(), CancellationToken);
            }
        }
    }
}
