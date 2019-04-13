﻿using Microsoft.AspNetCore.Identity;
using PrayerJournal.Core.Dtos;
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
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = await userManager.GetRolesAsync(user),
                ExternalLogins = (await userManager.GetLoginsAsync(user)).Select(l => l.LoginProvider).ToList()
            };
        }
    }
}