using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace PrayerJournal.Core.Filters
{
    public static class FindEntityFilterExtensions
    {
        public static THttpEntity GetFoundEntity<THttpEntity>(this HttpContext httpContext) where THttpEntity : class
        {
            return httpContext.Items["entity"] as THttpEntity;
        }

        public static IServiceCollection AddFindEntityFilter<TEntity>(this IServiceCollection services)
            where TEntity : class
        {
            return services.AddScoped<FindEntityFilter<TEntity>>();
        }
    }
}