using Microsoft.AspNetCore.Authentication;

namespace PrayerJournal.Authentication
{
    public class ShadyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }
    }
}
