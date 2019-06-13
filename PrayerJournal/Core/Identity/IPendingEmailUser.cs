using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Identity
{
    public interface IPendingEmailUser
    {
        string PendingEmail { get; set; }
        string NormalizedPendingEmail { get; set; }
        string PendingPhoneNumber { get; set; }
    }
}
