﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ShadySoft.Authentication;
using PrayerJournal.Controllers.Dtos;
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

namespace PrayerJournal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ShadySoft.Authentication.SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ShadySoft.Authentication.SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var (tokenString, result) = await _signInManager.EmailPasswordSignInAsync(model.Email, model.Password, true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                return Ok(await GenerateSignInResultDtoAsync(user, tokenString));
            }

            return this.SignInFailure(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registration)
        {
            if (await _userManager.FindByNameAsync(registration.Email) != null)
                return this.IdentityFailure("EmailInUse", "Email already in use.");

            var user = new ApplicationUser
            {
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                UserName = "Email-" + registration.Email,
                Email = registration.Email
            };

            var result = await _userManager.CreateAsync(user, registration.Password);
            if (result.Succeeded)
            {
                await SendEmailTokenAsync(user);

                var token = _signInManager.SignIn(user);
                return Ok(await GenerateSignInResultDtoAsync(user, token));
            }

            return this.IdentityFailure(result);
        }

        [HttpGet("confirm-email/{email}")]
        public async Task<IActionResult> GetEmailConfirmationCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

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
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                var token =_signInManager.SignIn(user);
                return Ok(await GenerateSignInResultDtoAsync(user, token));
            }

            return this.IdentityFailure(result);
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwords)
        {
            var user = HttpContext.GetAuthorizedUser<ApplicationUser>();

            var result = await _userManager.ChangePasswordAsync(user, passwords.OldPassword, passwords.NewPassword);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            await _signInManager.SignOutAsync(user);

            user.SuggestPasswordChange = false;
            await _userManager.UpdateAsync(user);

            var tokenString = _signInManager.SignIn(user);

            return Ok(await GenerateSignInResultDtoAsync(user, tokenString));
        }

        [HttpGet("password/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
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
            {
                await _signInManager.SignOutAsync(user);
                return Ok();
            }

            //Lie to the client if the token is invalid
            if (result.Errors.FirstOrDefault(e => e.Code == "InvalidToken") != null)
                return Ok();

            return this.IdentityFailure(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutEverywhere()
        {
            var user = HttpContext.GetAuthorizedUser<ApplicationUser>();

            await _signInManager.SignOutAsync(user);

            return Ok();
        }

        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([Required]string code, [Required]string provider)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync(code, provider);

            if (info == null)
                return this.IdentityFailure("ExternalLoginFailure", "Failed to get login info from external service.");

            var (tokenString, user, result) = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, bypassTwoFactor: true);
            if (result.Succeeded)
                return Ok(await GenerateSignInResultDtoAsync(user, tokenString));

            if (result.IsLockedOut)
                return this.SignInFailure(result);

            //User doesn't exist, create one
            user = CreateUserFromExternalPrincipalClaims(info);

            var userManagerResult = await _userManager.CreateAsync(user);
            if (userManagerResult.Succeeded)
            {
                userManagerResult = await _userManager.AddLoginAsync(user, info);
                if (userManagerResult.Succeeded)
                {
                    tokenString = _signInManager.SignIn(user);
                    return Ok(await GenerateSignInResultDtoAsync(user, tokenString));
                }
            }

            return this.IdentityFailure(userManagerResult);
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

            var callbackUrl = $"{Request.Scheme}://{Request.Host}/confirm-email?userId={Uri.EscapeDataString(user.Id)}&code={Uri.EscapeDataString(Uri.EscapeDataString(code))}";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private async Task<SignInResultsDto> GenerateSignInResultDtoAsync(ApplicationUser user, string token)
        {
            var responseObject = new SignInResultsDto
            {
                Token = token,
                UserId = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                Roles = await _userManager.GetRolesAsync(user)
            };

            if (user.SuggestPasswordChange)
                responseObject.Caveat = "ChangePassword";

            return responseObject;
        }

        private ApplicationUser CreateUserFromExternalPrincipalClaims(ExternalLoginInfo info)
        {
            return new ApplicationUser
            {
                FirstName = info.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ??
                    info.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.Split(" ")[0] ?? "",
                LastName = info.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value ??
                    info.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.Split(" ").Last() ?? "",
                UserName = info.LoginProvider + "-" + info.Principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value
            };
        }
    }
}