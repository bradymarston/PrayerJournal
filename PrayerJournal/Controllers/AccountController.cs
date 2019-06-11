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

namespace PrayerJournal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller, IControllerWithUserManager<ApplicationUser>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly OAuthService _oAuthService;

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            OAuthService oAuthService
            )
        {
            UserManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _oAuthService = oAuthService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.Remember, true);

            if (result.Succeeded)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                return Ok(await GenerateSignInResultDtoAsync(user));
            }

            return this.SignInFailure(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registration)
        {
            if (await UserManager.FindByNameAsync(registration.Email) != null)
                return this.IdentityFailure("EmailInUse", "Email already in use.");

            var user = new ApplicationUser
            {
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                UserName = "Email-" + registration.Email,
                Email = registration.Email
            };

            var result = await UserManager.CreateAsync(user, registration.Password);
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
            var user = await UserManager.FindByEmailAsync(email);

            //Don't reveal that the user doesn't exist or has already confirmed their email address
            if (user == null)
            {
                return Ok();
            }

            if (user.EmailConfirmed)
            {
                return Ok();
            }

            await SendEmailTokenAsync(user);
            return Ok();
        }

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([Required] string userId, [Required] string code)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await UserManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok(await GenerateSignInResultDtoAsync(user));
            }

            return this.IdentityFailure(result);
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwords)
        {
            var user = await this.GetAuthorizedUser();

            if (user == null)
                return this.IdentityFailure("MissingUser", "User no longer exists");

            var result = await UserManager.ChangePasswordAsync(user, passwords.OldPassword, passwords.NewPassword);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            user.SuggestPasswordChange = false;
            await UserManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        [HttpGet("password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user)))
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
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok();
            }

            var result = await UserManager.ResetPasswordAsync(user, code, model.Password);
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

            var user = await UserManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                //User doesn't exist, create one
                user = CreateUserFromExternalPrincipalClaims(info.Principal, provider);

                var userManagerResult = await UserManager.CreateAsync(user);
                if (!userManagerResult.Succeeded)
                    return this.IdentityFailure(userManagerResult);

                userManagerResult = await UserManager.AddLoginAsync(user, info);
                if (!userManagerResult.Succeeded)
                {
                    await UserManager.DeleteAsync(user);
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
            var user = await this.GetAuthorizedUser();

            if (user == null)
                return this.IdentityFailure("MissingUser", "User no longer exists");

            return Ok(await user.ToDtoAsync(UserManager));
        }

        private async Task SendPasswordTokenAsync(ApplicationUser user)
        {
            var code = await UserManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = $"{Request.Scheme}://{Request.Host}/reset-password?code={Uri.EscapeDataString(Uri.EscapeDataString(code))}";

            await _emailSender.SendEmailAsync(user.Email, "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private async Task SendEmailTokenAsync(ApplicationUser user)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = $"{Request.Scheme}://{Request.Host}/confirm-email?userId={Uri.EscapeDataString(user.Id)}&code={Uri.EscapeDataString(Uri.EscapeDataString(code))}";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private async Task<SignInResultsDto> GenerateSignInResultDtoAsync(ApplicationUser user)
        {
            var responseObject = new SignInResultsDto
            {
                HasPassword = user.PasswordHash != null,
                Name = $"{user.FirstName} {user.LastName}",
                Roles = await UserManager.GetRolesAsync(user)
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