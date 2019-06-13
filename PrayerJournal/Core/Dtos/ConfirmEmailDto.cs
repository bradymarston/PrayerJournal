using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Dtos
{
    public class ConfirmEmailDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
