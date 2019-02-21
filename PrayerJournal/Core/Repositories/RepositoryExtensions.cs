using Microsoft.Extensions.DependencyInjection;

namespace PrayerJournal.Core.Repositories
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepository<TEntity, TEntityRepository>(this IServiceCollection services) 
            where TEntity : class
            where TEntityRepository : class, IRepository<TEntity> 
        {
            return services.AddScoped<IRepository<TEntity>, TEntityRepository>();
        }
        public static IServiceCollection AddRepository<TEntity, TIEntityRepository, TEntityRepository>(this IServiceCollection services) 
            where TEntity : class
            where TIEntityRepository : class, IRepository<TEntity>
            where TEntityRepository : class, TIEntityRepository 
        {
            services.AddScoped<TIEntityRepository, TEntityRepository>();
            return services.AddRepository<TEntity, TEntityRepository>();
        }

        public static IServiceCollection AddUnitOfWork<TUnitOfWorkImplementation>(this IServiceCollection services)
            where TUnitOfWorkImplementation : class, IUnitOfWork
        {
            return services.AddScoped<IUnitOfWork, TUnitOfWorkImplementation>();
        }
    }
}