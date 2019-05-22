using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity item);
        void Remove(TEntity item);
        Task<TEntity> GetAsync(int id);
        Task<IEnumerable<TEntity>> GetAsync();
    }
}