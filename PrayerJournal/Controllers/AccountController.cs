using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PrayerJournal.Controllers.Extensions;
using PrayerJournal.Core;
using PrayerJournal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using PrayerJournal.Core.Filters;
using PrayerJournal.Core.Mappers;
using Microsoft.EntityFrameworkCore;
using PrayerJournal.Core.Dtos;
using ShadySoft.OAuth;
using PrayerJournal.Core.Identity;

namespace PrayerJournal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly OAuthService _oAuthService;
        private readonly PendingEmailUserManager<ApplicationUser> _userManager;

        public AccountController(
            PendingEmailUserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            OAuthService oAuthService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _oAuthService = oAuthService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                user = await _userManager.FindByPendingEmailAsync(model.Email);

                if (user == null)
                    return this.SignInFailure(Microsoft.AspNetCore.Identity.SignInResult.Failed);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.Remember, true);

            if (result.Succeeded)
            {
                return Ok(await GenerateSignInResultDtoAsync(user));
            }

            return this.SignInFailure(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registration)
        {
            if (await _userManager.FindByEmailAsync(registration.Email) != null)
                return this.IdentityFailure("EmailInUse", "Email already in use.");

            var user = new ApplicationUser
            {
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                UserName = Guid.NewGuid().ToString(),
                PendingEmail = registration.Email
            };

            var result = await _userManager.CreateAsync(user, registration.Password);
            if (result.Succeeded)
            {
                await SendEmailTokenAsync(user);

                await _signInManager.SignInAsync(user, false);
                return Ok(await GenerateSignInResultDtoAsync(user));
            }

            return this.IdentityFailure(result);
        }

        [HttpGet("confirm-email/{email}")]
        public async Task<IActionResult> GetEmailConfirmationCode(string email)
        {
            var user = await _userManager.FindByPendingEmailAsync(email);

            //Don't reveal that the user doesn't exist or has already confirmed their email address
            if (user == null)
            {
                return Ok();
            }

            await SendEmailTokenAsync(user);
            return Ok();
        }

        [HttpGet("confirm-email")]
        [Authorize]
        public async Task<IActionResult> GetEmailConfirmationCode()
        {
            var user = await _userManager.GetUserAsync(User);

            if (!await _userManager.HasPendingEmailAsync(user))
                return Ok();

            await SendEmailTokenAsync(user);
            return Ok();
        }

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var pendingEmail = await _userManager.GetPendingEmailAsync(user);

            if (string.IsNullOrWhiteSpace(pendingEmail))
                return this.IdentityFailure("NoPendignEmail", "There is no email address change to confirm.");

            if (await _userManager.FindByEmailAsync(pendingEmail) != null)
                return this.IdentityFailure("EmailInUse", "Email already in use.");

            var tokenResponse = await _userManager.VerifyEmailConfirmationToken(user, model.Code);
            if (!tokenResponse.Succeeded)
            {
                return this.IdentityFailure(tokenResponse);
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return this.IdentityFailure("IncorrectPasword", "Incorrect password.");

            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok(await GenerateSignInResultDtoAsync(user));
            }

            return this.IdentityFailure(result);
        }

        [HttpDelete("confirm-email")]
        [Authorize]
        public async Task<IActionResult> CancelEmailChange()
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ClearPendingEmailAsync(user);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            return Ok();
        }

        [HttpPost("email/{email}")]
        [Authorize]
        public async Task<IActionResult> SetEmail([EmailAddress]string email)
        {
            var user = await _userManager.GetUserAsync(User);

            if ((await _userManager.GetLoginsAsync(user)).Count > 0)
                return this.IdentityFailure("ExternalUsersProhibited", "External users cannot change their email address.");

            if (await _userManager.FindByEmailAsync(email) != null)
                return this.IdentityFailure("EmailInUse", "This email address is already in use");

            var result = await _userManager.SetPendingEmailAsync(user, email);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            await SendEmailTokenAsync(user);

            return Ok();
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwords)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return this.IdentityFailure("MissingUser", "User no longer exists");

            var result = await _userManager.ChangePasswordAsync(user, passwords.OldPassword, passwords.NewPassword);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            user.SuggestPasswordChange = false;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        [HttpGet("password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Ok();
            }

            await SendPasswordTokenAsync(user);

            return Ok();
        }

        [HttpPost("password")]
        public async Task<IActionResult> ResetPassword([Required] string code, ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok();
            }

            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
            if (result.Succeeded)
                return Ok();

            //Lie to the client if the token is invalid
            if (result.Errors.FirstOrDefault(e => e.Code == "InvalidToken") != null)
                return Ok();

            return this.IdentityFailure(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutEverywhere()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }

        [HttpGet("external-login/{provider}")]
        public IActionResult GetExternalLoginUrl(string provider)
        {
            return Ok(new ExternalLoginUrlDto
            {
                Url = _oAuthService.BuildChallengeUrl(provider)
            });
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([Required]string code, [Required]string state, [Required]string provider)
        {
            ExternalLoginInfo info;

            try
            {
                info = await _oAuthService.GetExternalLoginInfo(provider, code, state);
            }
            catch (Exception)
            {
                return this.IdentityFailure("ExternalLoginFailure", "Failed to get login info from external service.");
            }

            if (info == null)
                return this.IdentityFailure("ExternalLoginFailure", "Failed to get login info from external service.");

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                //User doesn't exist, create one
                user = CreateUserFromExternalPrincipalClaims(info.Principal, provider);

                var userManagerResult = await _userManager.CreateAsync(user);
                if (!userManagerResult.Succeeded)
                    return this.IdentityFailure(userManagerResult);

                userManagerResult = await _userManager.AddLoginAsync(user, info);
                if (!userManagerResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return this.IdentityFailure(userManagerResult);
                }
            }

            await _signInManager.SignInAsync(user, false);
            return Ok(await GenerateSignInResultDtoAsync(user));
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return this.IdentityFailure("MissingUser", "User no longer exists");

            return Ok(await user.ToDtoAsync(_userManager));
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UserDto userDto)
        {
            var userInDb = await _userManager.GetUserAsync(User);

            if (userInDb == null)
                return this.IdentityFailure("MissingUser", "User no longer exists");

            userDto.UpdateData(userInDb);

            await _userManager.UpdateAsync(userInDb);

            await _signInManager.RefreshSignInAsync(userInDb);

            return Ok(await GenerateSignInResultDtoAsync(userInDb));
        }

        private async Task SendPasswordTokenAsync(ApplicationUser user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = $"{Request.Scheme}://{Request.Host}/reset-password?code={Uri.EscapeDataString(Uri.EscapeDataString(code))}";

            await _emailSender.SendEmailAsync(user.Email, "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private async Task SendEmailTokenAsync(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = $"{Request.Scheme}://{Request.Host}/confirm-email?userId={Uri.EscapeDataString(user.Id)}&code={Uri.EscapeDataString(code)}";

            await _emailSender.SendEmailAsync(user.PendingEmail, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private async Task<SignInResultsDto> GenerateSignInResultDtoAsync(ApplicationUser user)
        {
            var responseObject = new SignInResultsDto
            {
                HasPassword = user.PasswordHash != null,
                Name = $"{user.FirstName} {user.LastName}",
                Roles = await _userManager.GetRolesAsync(user)
            };

            if (user.SuggestPasswordChange)
                responseObject.Caveat = "ChangePassword";

            return responseObject;
        }

        private ApplicationUser CreateUserFromExternalPrincipalClaims(ClaimsPrincipal principal, string provider)
        {
            return new ApplicationUser
            {
                FirstName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ??
                    principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.Split(" ")[0] ?? "",
                LastName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ??
                    principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.Split(" ").Last() ?? "",
                UserName = provider + "-" + principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            };
        }
    }
}