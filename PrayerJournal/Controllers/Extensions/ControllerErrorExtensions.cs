using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerJournal.Controllers.Extensions
{
    public static class ControllerErrorExtensions
    {
        public static IActionResult IdentityFailure(this ControllerBase controller, IdentityResult result)
        {
            return controller.BadRequest(GenerateProblemDetails(result));
        }

        public static IActionResult BadModel(this ControllerBase controller, string key, string errorMessage)
        {
            controller.ModelState.AddModelError(key, errorMessage);
            return controller.BadModel();
        }

        public static IActionResult BadModel(this ControllerBase controller)
        {
            return controller.BadRequest(GenerateProblemDetails(controller.ModelState));
        }

        public static IServiceCollection ConfigureAutomaticValidationResponse(this IServiceCollection services)
        {
            return services.Configure<ApiBehaviorOptions>(options =>
                options.InvalidModelStateResponseFactory = context =>
                    new BadRequestObjectResult(GenerateProblemDetails(context.ModelState)));
        }

        private static ProblemDetails GenerateProblemDetails(ModelStateDictionary state)
        {
            var problemDetails = new ProblemDetails()
            {
                Type = "ValidationError",
                Title = "One or more validation errors occurred."
            };

            problemDetails.AddErrors(state.Select(p => new ErrorItem() {
                Key = string.IsNullOrEmpty(p.Key) ? "" : char.ToLower(p.Key[0]) + p.Key.Substring(1),
                Descriptions = p.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            }));

            return problemDetails;
        }

        private static ProblemDetails GenerateProblemDetails(IdentityResult result)
        {
            var problemDetails = new ProblemDetails()
            {
                Type = "IdentityError",
                Title = "One or more errors occurred in the identity system."
            };

            problemDetails.AddErrors(result.Errors.Select(e => new ErrorItem() {
                Key = e.Code,
                Descriptions = new string[] { e.Description }
            }));

            return problemDetails;
        }

        private static void AddErrors(this ProblemDetails problemDetails, IEnumerable<ErrorItem> errors)
        {
            problemDetails.Extensions.Add("errors", new Dictionary<string, string[]>());

            if (errors.Count() > 0)
                foreach (var error in errors)
                {
                    ((Dictionary<string, string[]>)problemDetails.Extensions["errors"]).Add(error.Key, error.Descriptions );
                }
        }

        private class ErrorItem
        {
            public string Key { get; set; }
            public string[] Descriptions { get; set; }
        }
    }
}