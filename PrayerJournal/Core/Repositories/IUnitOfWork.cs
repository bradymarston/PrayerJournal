using System.Threading.Tasks;

namespace PrayerJournal.Core.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}