using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PrayerJournal.Authentication.Models;

namespace PrayerJournal.Authentication
{
    public interface IShadySignInManager<TUser> where TUser : IdentityUser, IShadyUser
    {
        IdentityOptions Options { get; }

        Task<bool> CanSignInAsync(TUser user);
        Task<ShadySignInResult> CheckPasswordSignInAsync(TUser user, string password, bool lockoutOnFailure);
        Task<bool> IsTwoFactorClientRememberedAsync(TUser user);
        Task<ShadySignInResult> PasswordSignInAsync(string userName, string password, bool lockoutOnFailure);
        Task<ShadySignInResult> PasswordSignInAsync(TUser user, string password, bool lockoutOnFailure);
        string SignIn(TUser user);
        Task SignOutAsync(TUser user);
    }
}