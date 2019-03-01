using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using PrayerJournal.Authentication.Models;

namespace PrayerJournal.Authentication
{
    public class ShadyTokenService : IShadyTokenService
    {
        private readonly IDataProtectionProvider _protectionProvider;

        public ShadyTokenService(IDataProtectionProvider protectionProvider)
        {
            _protectionProvider = protectionProvider;
        }

        public ShadyToken DecodeTokenString(string tokenString)
        {
            var decryptedTokenString = "";

            try
            {
                var protector = _protectionProvider.CreateProtector("UserToken");
                decryptedTokenString = protector.Unprotect(tokenString);
            }
            catch (CryptographicException)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(decryptedTokenString))
                return null;

            return JsonConvert.DeserializeObject<ShadyToken>(decryptedTokenString);
        }

        public string GenerateTokenString(string userName)
        {
            var token = new ShadyToken() { UserName = userName, Issued = DateTime.UtcNow };

            var protector = _protectionProvider.CreateProtector("UserToken");
            return protector.Protect(JsonConvert.SerializeObject(token));
        }
    }
}
