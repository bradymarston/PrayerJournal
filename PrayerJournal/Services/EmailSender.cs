using System.Diagnostics;
using System.Threading.Tasks;
using PrayerJournal.Core;

namespace PrayerJournal.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Debug.WriteLine("************************************************");
            Debug.WriteLine($"Recipient: {email}");
            Debug.WriteLine("************************************************");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine("************************************************");
            Debug.WriteLine(htmlMessage);

            return Task.CompletedTask;
        }
    }
}
