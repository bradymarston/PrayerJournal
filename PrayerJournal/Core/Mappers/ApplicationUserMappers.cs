using Microsoft.AspNetCore.Identity;
using PrayerJournal.Controllers.Dtos;
using PrayerJournal.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Mappers
{
    public static class ApplicationUserMappers
    {
        public static async Task<UserDto> ToDtoAsync(this ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            return new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastNama = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = await userManager.GetRolesAsync(user)
            };
        }
    }
}