using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PrayerJournal.Controllers.Dtos;
using PrayerJournal.Controllers.Extensions;
using PrayerJournal.Core.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<object> Login([FromBody] LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                return GenerateJwtToken(model.Email, appUser);
            }

            if (result.IsLockedOut)
                return this.BadModel("Password", "Your account has been locked out. Please try again in a few minutes.");

            return this.BadModel("", "Incorrect email or password.");
        }

        [HttpPost("register")]
        public async Task<object> Register([FromBody] RegisterDto registration)
        {
            if (await _userManager.FindByNameAsync(registration.Email) != null)
                return this.BadModel("Email", "Email already in use.");

            var user = new ApplicationUser
            {
                UserName = registration.Email,
                Email = registration.Email
            };

            var result = await _userManager.CreateAsync(user, registration.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return GenerateJwtToken(registration.Email, user);
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

            if (result.Succeeded)
                return Ok();

            return this.IdentityFailure(result);
        }

        private object GenerateJwtToken(string email, IdentityUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
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
    }
}
