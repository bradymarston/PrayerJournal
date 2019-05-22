using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using PrayerJournal.Core.Repositories;

namespace PrayerJournal.Persistence
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        virtual public void Add(TEntity item)
        {
            _context.Add(item);
        }

        virtual public async Task<TEntity> GetAsync(int id)
        {
            return await _context.FindAsync<TEntity>(id);
        }

        virtual public async Task<IEnumerable<TEntity>> GetAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        virtual public void Remove(TEntity item)
        {
            _context.Remove(item);
        }
    }
}
