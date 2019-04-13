using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Dtos
{
    public class SignInResultsDto
    {
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public bool HasPassword { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public string? Caveat { get; set; }
        public string? Token { get; set; }
    }
}
