﻿using Microsoft.AspNetCore.Identity;
using ShadySoft.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        public bool SuggestPasswordChange { get; set; }
    }
}
