using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PrayerJournal.Controllers.Dtos;
using PrayerJournal.Controllers.Extensions;
using PrayerJournal.Core;
using PrayerJournal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);

            if (result.Succeeded)
            {
                return Ok(GenerateSignInResultDto(model.Email));
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

                await _signInManager.SignInAsync(user, false);
                return Ok(GenerateSignInResultDto(user.UserName));
            }

            return this.IdentityFailure(result);
        }

        [HttpGet("confirm-email")]
        [Authorize]
        public async Task<IActionResult> GetEmailConfirmationCode()
        {
            var userName = HttpContext.GetCurrentUserName();
            var user = await _userManager.FindByNameAsync(userName);

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
                await _signInManager.SignInAsync(user, false);
                return Ok(GenerateSignInResultDto(user.UserName));
            }

            return this.IdentityFailure(result);
        }

        [HttpPut("password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwords)
        {
            var userName = HttpContext.GetCurrentUserName();
            var user = await _userManager.FindByNameAsync(userName);

            var result = await _userManager.ChangePasswordAsync(user, passwords.OldPassword, passwords.NewPassword);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            if (user.SuggestPasswordChange)
            {
                user.SuggestPasswordChange = false;
                await _userManager.UpdateAsync(user);
            }

            return Ok(GenerateSignInResultDto(user.UserName));
        }

        private string GenerateJwtToken(string userName, IdentityUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task SendEmailTokenAsync(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = $"{Request.Scheme}://{Request.Host}/confirm-email?userId={Uri.EscapeDataString(user.Id)}&code={Uri.EscapeDataString(Uri.EscapeDataString(code))}";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }

        private SignInResultsDto GenerateSignInResultDto(string userName)
        {
            var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == userName);
            var responseObject = new SignInResultsDto
            {
                Token = GenerateJwtToken(userName, appUser),
                UserName = userName
            };

            if (appUser.SuggestPasswordChange)
                responseObject.Caveat = "ChangePassword";

            if (!appUser.EmailConfirmed)
                responseObject.Caveat = "ConfirmEmail";

            return responseObject;
        }
    }
}