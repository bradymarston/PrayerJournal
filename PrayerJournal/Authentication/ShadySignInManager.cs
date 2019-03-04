﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PrayerJournal.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrayerJournal.Authentication
{
    public class ShadySignInManager<TUser> : IShadySignInManager<TUser> where TUser : IdentityUser, IShadyUser
    {
        private readonly UserManager<TUser> _userManager;
        private readonly ILogger<SignInManager<TUser>> _logger;
        private readonly IShadyTokenService _tokenService;

        public IdentityOptions Options { get; }

        public ShadySignInManager(UserManager<TUser> userManager,
                                  IOptions<IdentityOptions> optionsAccessor,
                                  ILogger<SignInManager<TUser>> logger,
                                  IShadyTokenService tokenService)
        {
            _userManager = userManager;
            _logger = logger;
            _tokenService = tokenService;
            Options = optionsAccessor?.Value ?? new IdentityOptions();
        }

        /// <summary>
        /// Returns a flag indicating whether the specified user can sign in.
        /// </summary>
        /// <param name="user">The user whose sign-in status should be returned.</param>
        /// <returns>
        /// The task object representing the asynchronous operation, containing a flag that is true
        /// if the specified user can sign-in, otherwise false.
        /// </returns>
        public virtual async Task<bool> CanSignInAsync(TUser user)
        {
            if (Options.SignIn.RequireConfirmedEmail && !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                _logger.LogWarning(0, "User {userId} cannot sign in without a confirmed email.", await _userManager.GetUserIdAsync(user));
                return false;
            }
            if (Options.SignIn.RequireConfirmedPhoneNumber && !(await _userManager.IsPhoneNumberConfirmedAsync(user)))
            {
                _logger.LogWarning(1, "User {userId} cannot sign in without a confirmed phone number.", await _userManager.GetUserIdAsync(user));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Signs in the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to sign-in.</param>
        /// <param name="authenticationProperties">Properties applied to the login and authentication cookie.</param>
        /// <param name="authenticationMethod">Name of the method used to authenticate the user.</param>
        /// <returns>Valid authentication token.</returns>
        public virtual string SignIn(TUser user)
        {
            return _tokenService.GenerateTokenString(user.UserName);
        }

        /// <summary>
        /// Invalidates all user tokens before issued before this time.
        /// </summary>
        /// <param name="user">The the user to sign-out.</param>
        public virtual async Task SignOutAsync(TUser user)
        {
            user.TokensInvalidBefore = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Attempts to sign in the specified <paramref name="user"/> and <paramref name="password"/> combination
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        public virtual async Task<ShadySignInResult> PasswordSignInAsync(TUser user, string password,
            bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var attempt = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);
            return attempt.Succeeded
                ? await SignInOrTwoFactorAsync(user)
                : attempt;
        }

        /// <summary>
        /// Attempts to sign in the specified <paramref name="userName"/> and <paramref name="password"/> combination
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="userName">The user name to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        public virtual async Task<ShadySignInResult> PasswordSignInAsync(string userName, string password,
            bool lockoutOnFailure)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return ShadySignInResult.Failed;
            }

            return await PasswordSignInAsync(user, password, lockoutOnFailure);
        }

        /// <summary>
        /// Attempts a password sign in for a user.
        /// </summary>
        /// <param name="user">The user to sign in.</param>
        /// <param name="password">The password to attempt to sign in with.</param>
        /// <param name="lockoutOnFailure">Flag indicating if the user account should be locked if the sign in fails.</param>
        /// <returns>The task object representing the asynchronous operation containing the <see name="SignInResult"/>
        /// for the sign-in attempt.</returns>
        /// <returns></returns>
        public virtual async Task<ShadySignInResult> CheckPasswordSignInAsync(TUser user, string password, bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var error = await PreSignInCheck(user);
            if (error != null)
            {
                return error;
            }

            if (await _userManager.CheckPasswordAsync(user, password))
            {
                var alwaysLockout = AppContext.TryGetSwitch("Microsoft.AspNetCore.Identity.CheckPasswordSignInAlwaysResetLockoutOnSuccess", out var enabled) && enabled;
                // Only reset the lockout when TFA is not enabled when not in quirks mode
                if (alwaysLockout || !await IsTfaEnabled(user))
                {
                    await ResetLockout(user);
                }

                return ShadySignInResult.Success;
            }
            _logger.LogWarning(2, "User {userId} failed to provide the correct password.", await _userManager.GetUserIdAsync(user));

            if (_userManager.SupportsUserLockout && lockoutOnFailure)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await _userManager.AccessFailedAsync(user);
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return await LockedOutAsync(user);
                }
            }
            return ShadySignInResult.Failed;
        }

        /// <summary>
        /// Returns a flag indicating if the current client browser has been remembered by two factor authentication
        /// for the user attempting to login, as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user attempting to login.</param>
        /// <returns>
        /// The task object representing the asynchronous operation containing true if the browser has been remembered
        /// for the current user.
        /// </returns>
        public virtual async Task<bool> IsTwoFactorClientRememberedAsync(TUser user)
        {
            return false;
        }

        private async Task<bool> IsTfaEnabled(TUser user)
            => _userManager.SupportsUserTwoFactor &&
            await _userManager.GetTwoFactorEnabledAsync(user) &&
            (await _userManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;

        /// <summary>
        /// Signs in the specified <paramref name="user"/> if <paramref name="bypassTwoFactor"/> is set to false.
        /// Otherwise stores the <paramref name="user"/> for use after a two factor check.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isPersistent">Flag indicating whether the sign-in cookie should persist after the browser is closed.</param>
        /// <param name="loginProvider">The login provider to use. Default is null</param>
        /// <param name="bypassTwoFactor">Flag indicating whether to bypass two factor authentication. Default is false</param>
        /// <returns>Returns a <see cref="SignInResult"/></returns>
        protected virtual async Task<ShadySignInResult> SignInOrTwoFactorAsync(TUser user, string loginProvider = null, bool bypassTwoFactor = false)
        {
            if (!bypassTwoFactor && await IsTfaEnabled(user))
            {
                if (!await IsTwoFactorClientRememberedAsync(user))
                {
                    // Store the userId for use after two factor check
                }
            }
            return new ShadySignInResult(SignIn(user));
        }

        /// <summary>
        /// Used to determine if a user is considered locked out.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Whether a user is considered locked out.</returns>
        protected virtual async Task<bool> IsLockedOut(TUser user)
        {
            return _userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user);
        }

        /// <summary>
        /// Returns a locked out SignInResult.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>A locked out SignInResult</returns>
        protected virtual async Task<ShadySignInResult> LockedOutAsync(TUser user)
        {
            _logger.LogWarning(3, "User {userId} is currently locked out.", await _userManager.GetUserIdAsync(user));
            return ShadySignInResult.LockedOut;
        }

        /// <summary>
        /// Used to ensure that a user is allowed to sign in.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>Null if the user should be allowed to sign in, otherwise the SignInResult why they should be denied.</returns>
        protected virtual async Task<ShadySignInResult> PreSignInCheck(TUser user)
        {
            if (!await CanSignInAsync(user))
            {
                return ShadySignInResult.NotAllowed;
            }
            if (await IsLockedOut(user))
            {
                return await LockedOutAsync(user);
            }
            return null;
        }

        /// <summary>
        /// Used to reset a user's lockout count.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
        protected virtual Task ResetLockout(TUser user)
        {
            if (_userManager.SupportsUserLockout)
            {
                return _userManager.ResetAccessFailedCountAsync(user);
            }
            return Task.CompletedTask;
        }
    }
}