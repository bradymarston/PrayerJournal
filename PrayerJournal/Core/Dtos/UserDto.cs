using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Dtos
{
    public class UserDto
    {
        public string Id { get; set; } = "";
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool HasPassword { get; set; }
        public string PendingEmail { get; set; }
        public string PendingPhoneNumber { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public IEnumerable<string> ExternalLogins { get; set; } = new List<string>();
    }
}
