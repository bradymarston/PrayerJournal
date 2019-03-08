﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PrayerJournal.Authentication;
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

namespace PrayerJournal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IShadySignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IShadyTokenService _tokenService;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IShadySignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IShadyTokenService tokenService,
            IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true);

            if (result.Succeeded)
            {
                return Ok(await GenerateSignInResultDtoAsync(model.Email, result.Token));
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
                UserName = registration.Email,
                Email = registration.Email
            };

            var result = await _userManager.CreateAsync(user, registration.Password);
            if (result.Succeeded)
            {
                await SendEmailTokenAsync(user);

                var token = _signInManager.SignIn(user);
                return Ok(await GenerateSignInResultDtoAsync(user.UserName, token));
            }

            return this.IdentityFailure(result);
        }

        [HttpGet("confirm-email")]
        [Authorize]
        public async Task<IActionResult> GetEmailConfirmationCode()
        {
            var user = this.GetAuthenticatedUser<ApplicationUser>();

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return this.IdentityFailure("MissingEmail", "User does not have an email address.");
            }

            if (user.EmailConfirmed)
            {
                return this.IdentityFailure("EmailConfirmed", "Email already confrimed.");
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
                return Ok(await GenerateSignInResultDtoAsync(user.UserName, token));
            }

            return this.IdentityFailure(result);
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwords)
        {
            var user = this.GetAuthenticatedUser<ApplicationUser>();

            var result = await _userManager.ChangePasswordAsync(user, passwords.OldPassword, passwords.NewPassword);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            if (user.SuggestPasswordChange)
            {
                user.SuggestPasswordChange = false;
                await _userManager.UpdateAsync(user);
            }

            return Ok(await GenerateSignInResultDtoAsync(user.UserName, _tokenService.GenerateTokenString(user.UserName)));
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
                return Ok();
            }

            //Lie to the client if the token is invalid
            if (result.Errors.FirstOrDefault(e => e.Code == "InvalidToken") != null)
                return Ok();

            return this.IdentityFailure(result);
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

        private async Task<SignInResultsDto> GenerateSignInResultDtoAsync(string userName, string token)
        {
            var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == userName);
            var responseObject = new SignInResultsDto
            {
                Token = token,
                UserName = userName
            };

            if (appUser.SuggestPasswordChange)
                responseObject.Caveat = "ChangePassword";

            if (!await _userManager.IsEmailConfirmedAsync(appUser))
                responseObject.Caveat = "ConfirmEmail";

            return responseObject;
        }
    }
}