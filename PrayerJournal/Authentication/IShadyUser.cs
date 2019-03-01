using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Authentication
{
    public interface IShadyUser
    {
        DateTime TokensInvalidBefore { get; set; }
    }
}
