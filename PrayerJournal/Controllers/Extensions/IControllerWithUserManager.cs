using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Controllers.Extensions
{
    public interface IControllerWithUserManager<TUser> where TUser : IdentityUser
    {
        public UserManager<TUser> UserManager { get; }
        public HttpContext HttpContext { get; }
    }
}
