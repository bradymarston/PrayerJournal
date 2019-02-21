using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PrayerJournal.Core.Repositories;
using System.Threading.Tasks;

namespace PrayerJournal.Core.Filters
{
    public class FindEntityFilter<TEntity> : IAsyncActionFilter where TEntity : class
    {
        private readonly IRepository<TEntity> _repository;

        public FindEntityFilter(IRepository<TEntity> repository)
        {
            _repository = repository;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
        {
            if (actionContext.ActionArguments.ContainsKey("id"))
            {
                var entity = await _repository.GetAsync((int)actionContext.ActionArguments["id"]);
                if (entity is null)
                {
                    actionContext.Result = new NotFoundResult();
                    return;
                }

                actionContext.HttpContext.Items.Add("entity", entity);
            }

            await next();
        }
    }
}
