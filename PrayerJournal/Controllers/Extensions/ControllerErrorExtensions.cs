using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using PrayerJournal.Authentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PrayerJournal.Controllers.Extensions
{
    public static class ControllerErrorExtensions
    {
        public static IActionResult SignInFailure(this ControllerBase controller, ShadySignInResult result)
        {
            var problemDetails = GenerateProblemDetails(result);

            if ((string)problemDetails.Extensions["Reason"] == "InvalidSignIn")
            {
                return controller.BadRequest(problemDetails);
            }

            return controller.Unauthorized(problemDetails);
        }

        public static IActionResult IdentityFailure(this ControllerBase controller, IdentityResult result)
        {
            return controller.BadRequest(GenerateProblemDetails(result));
        }

        public static IActionResult IdentityFailure(this ControllerBase controller, string code, string description)
        {
            var result = IdentityResult.Failed(new IdentityError() { Code = code, Description = description });

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

        private static ProblemDetails GenerateProblemDetails(ShadySignInResult result)
        {
            var problemDetails = new ProblemDetails()
            {
                Type = "SignInError",
                Title = "One or more errors during sign-in."
            };

            var reason = "InvalidSignIn";

            if (result.RequiresTwoFactor)
                reason = "TwoFactor";

            if (result.IsLockedOut)
                reason = "LockedOut";

            if (result.ConfirmCredential)
                reason = "CredentialUnconfirmed";

            problemDetails.AddExtension("Reason", reason);

            if (reason == "InvalidSignIn")
                problemDetails.AddErrors(new List<ErrorItem>()
                {
                    new ErrorItem()
                    {
                        Key = "Credentials",
                        Descriptions = new string[] { "Invalid email address or password." }
                    }
                });

            return problemDetails;
        }

        private static void AddErrors(this ProblemDetails problemDetails, IEnumerable<ErrorItem> errors)
        {
            var errorExtension = new Dictionary<string, string[]>();

            if (errors.Count() > 0)
                foreach (var error in errors)
                {
                    errorExtension.Add(error.Key, error.Descriptions );
                }

            problemDetails.AddExtension("errors", errorExtension);
        }

        private static void AddExtension(this ProblemDetails problemDetails, string key, object data)
        {
            problemDetails.Extensions.Add(key, data);
        }

        private class ErrorItem
        {
            public string Key { get; set; }
            public string[] Descriptions { get; set; }
        }
    }
}