using Microsoft.AspNetCore.Identity;
using PrayerJournal.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Models
{
    public class ApplicationUser : IdentityUser, IShadyUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool SuggestPasswordChange { get; set; }
        public DateTime TokensInvalidBefore { get; set; }
    }
}
