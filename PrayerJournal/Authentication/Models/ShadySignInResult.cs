using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Authentication.Models
{
    public class ShadySignInResult : SignInResult
    {
        new public static ShadySignInResult Success => new ShadySignInResult(succeeded: true);
        new public static ShadySignInResult LockedOut => new ShadySignInResult(isLockedOut: true);
        new public static ShadySignInResult NotAllowed => new ShadySignInResult(isNotAllowed: true);
        new public static ShadySignInResult TwoFactorRequired => new ShadySignInResult(requiresTwoFactor: true);
        new public static ShadySignInResult Failed => new ShadySignInResult();

        public ShadySignInResult(string token = "", bool succeeded = false, bool isLockedOut = false, bool isNotAllowed = false, bool requiresTwoFactor = false)
        {
            Succeeded = !string.IsNullOrWhiteSpace(token) || succeeded;
            IsLockedOut = isLockedOut;
            IsNotAllowed = isNotAllowed;
            RequiresTwoFactor = requiresTwoFactor;
            Token = token;
        }

        public string Token { get; }
    }
}
