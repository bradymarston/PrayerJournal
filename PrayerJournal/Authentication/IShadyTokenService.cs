using PrayerJournal.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Authentication
{
    public interface IShadyTokenService
    {
        ShadyToken DecodeTokenString(string tokenString);
        string GenerateTokenString(string userName);
    }
}
