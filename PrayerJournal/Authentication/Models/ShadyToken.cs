using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Authentication.Models
{
    public class ShadyToken
    {
        public string UserName { get; set; }
        public DateTime Issued { get; set; }
    }
}
