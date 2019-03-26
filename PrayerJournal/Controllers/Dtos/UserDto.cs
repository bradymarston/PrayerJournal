﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Controllers.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastNama { get; set; } = "";
        public string? Email { get; set; } 
        public string? PhoneNumber { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
