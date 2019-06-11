using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrayerJournal.Controllers.Extensions;
using PrayerJournal.Core.Dtos;
using PrayerJournal.Core.Filters;
using PrayerJournal.Core.Mappers;
using PrayerJournal.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrayerJournal.Controllers
{
    [ApiController]
    [Route("api/user-admin")]
    public class UserAdminController : Controller, IControllerWithUserManager<ApplicationUser>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManager<ApplicationUser> UserManager { get; }

        public UserAdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = UserManager.Users.ToList();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(await user.ToDtoAsync(UserManager));
            }

            return Ok(userDtos);
        }

        [HttpDelete("user/{userId}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(FindUserFilter<ApplicationUser>))]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = HttpContext.GetFoundUser<ApplicationUser>();

            var result = await UserManager.DeleteAsync(user);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            return Ok();
        }

        [HttpGet("roles")]
        [Authorize]
        public async Task<IActionResult> GetAuthorizedUserRoles()
        {
            var authorizedUser = await this.GetAuthorizedUser();

            return Ok(await UserManager.GetRolesAsync(authorizedUser));
        }

        [HttpGet("roles/{userId}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(FindUserFilter<ApplicationUser>))]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = HttpContext.GetFoundUser<ApplicationUser>();

            if (user == null)
                return NotFound();

            return Ok(await UserManager.GetRolesAsync(user));
        }

        [HttpPost("roles/{userId}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(FindUserFilter<ApplicationUser>))]
        public async Task<IActionResult> AddRole(string userId, [Required] string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                return this.IdentityFailure("RoleDoesNotExist", "This role does not exist");

            var user = HttpContext.GetFoundUser<ApplicationUser>();

            var result = await UserManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            return Ok();
        }

        [HttpDelete("roles/{userId}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(FindUserFilter<ApplicationUser>))]
        public async Task<IActionResult> RemoveRole(string userId, [Required] string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                return this.IdentityFailure("RoleDoesNotExist", "This role does not exist");

            var authorizedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == authorizedUserId && role == "Admin")
                return this.IdentityFailure("CannotRemoveOwnAdminRole", "You cannot remove your own admin role.");

            var user = HttpContext.GetFoundUser<ApplicationUser>();

            var result = await UserManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            return Ok();
        }
    }
}