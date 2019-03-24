using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Controllers.Dtos
{
    public class SignInResultsDto
    {
        public string UserName { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Caveat { get; set; }
        public string? Token { get; set; }
    }
}
