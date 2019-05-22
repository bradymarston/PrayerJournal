using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrayerJournal.Persistence;
using PrayerJournal.Core.Repositories;
using PrayerJournal.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System;
using PrayerJournal.Controllers.Extensions;
using PrayerJournal.Core;
using PrayerJournal.Services;
using ShadySoft.Authentication;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using PrayerJournal.Services.Extensions;
using PrayerJournal.Core.Filters;
using ShadySoft.Authentication.OAuth;

namespace PrayerJournal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

            services.ConfigureAutomaticValidationResponse();

            services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(ShadyAuthenticationDefaults.AuthenticationScheme)
                .AddShady<ApplicationUser>(options => {
                    options.Realm = "PrayerJournal";
                    options.ExternalLoginProviders = new List<IOAuthHttpService>
                    {
                        new FacebookHttpService(
                            Configuration.GetExternalLoginClientId("Facebook"),
                            Configuration.GetExternalLoginClientSecret("Facebook"),
                            Configuration.GetExternalLoginCallbackUri()),
                        new GoogleHttpService(
                            Configuration.GetExternalLoginClientId("Google"),
                            Configuration.GetExternalLoginClientSecret("Google"),
                            Configuration.GetExternalLoginCallbackUri()),
                        new MicrosoftHttpService(
                            Configuration.GetExternalLoginClientId("Microsoft"),
                            Configuration.GetExternalLoginClientSecret("Microsoft"),
                            Configuration.GetExternalLoginCallbackUri())
                    };
                });

            services.AddUnitOfWork<UnitOfWork>();
            services.AddScoped<IEmailSender, EmailSender>();

            services.AddFindUserFilter<ApplicationUser>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();

            roleManager.EnsureRolesExist(new List<IdentityRole> { new IdentityRole { Name = "Admin" } });
            userManager.EnsureRoleUserExists("Admin", new ApplicationUser { FirstName = "Default", LastName = "Admin", Email = "admin@admin.com", UserName = "Admin" }, "Admin1!");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
