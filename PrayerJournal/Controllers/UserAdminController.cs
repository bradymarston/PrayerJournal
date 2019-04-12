using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrayerJournal.Controllers.Extensions;
using PrayerJournal.Core.Dtos;
using PrayerJournal.Core.Filters;
using PrayerJournal.Core.Mappers;
using PrayerJournal.Core.Models;
using ShadySoft.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Controllers
{
    [ApiController]
    [Route("api/user-admin")]
    public class UserAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserAdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = _userManager.Users.ToList();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(await user.ToDtoAsync(_userManager));
            }

            return Ok(userDtos);
        }

        [HttpDelete("user/{userId}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(FindUserFilter<ApplicationUser>))]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = HttpContext.GetFoundUser<ApplicationUser>();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            return Ok();
        }

        [HttpGet("roles")]
        [Authorize]
        public async Task<IActionResult> GetAuthorizedUserRoles()
        {
            var authorizedUser = HttpContext.GetAuthorizedUser<ApplicationUser>();

            return Ok(await _userManager.GetRolesAsync(authorizedUser));
        }

        [HttpGet("roles/{userId}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(FindUserFilter<ApplicationUser>))]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var user = HttpContext.GetFoundUser<ApplicationUser>();

            if (user == null)
                return NotFound();

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [HttpPost("roles/{userId}")]
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(FindUserFilter<ApplicationUser>))]
        public async Task<IActionResult> AddRole(string userId, [Required] string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                return this.IdentityFailure("RoleDoesNotExist", "This role does not exist");

            var user = HttpContext.GetFoundUser<ApplicationUser>();

            var result = await _userManager.AddToRoleAsync(user, role);

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

            var authorizedUser = HttpContext.GetAuthorizedUser<ApplicationUser>();

            if (userId == authorizedUser.Id && role == "Admin")
                return this.IdentityFailure("CannotRemoveOwnAdminRole", "You cannot remove your own admin role.");

            var user = HttpContext.GetFoundUser<ApplicationUser>();

            var result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
                return this.IdentityFailure(result);

            return Ok();
        }
    }
}