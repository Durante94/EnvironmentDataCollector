using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEnvironmentDataCollector.Data;
using WebEnvironmentDataCollector.Models;
using WebEnvironmentDataCollector.Util;

namespace WebEnvironmentDataCollector
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
            bool isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString(isDev ? "DevelopConnection" : "DefaultConnection")));
            services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages().AddNewtonsoftJson();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+?!";
                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedAccount = false;//true;
                options.SignIn.RequireConfirmedEmail = false;//true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });

            services.AddTransient<FileHandler>()
                .AddTransient<IEmailService, EmailService>(opt =>
                {
                    IConfigurationSection smtpSettings = Configuration.GetSection("SMTPSettings");
                    return new EmailService(
                         smtpSettings.GetValue<string>("SmtpHost"),
                         smtpSettings.GetValue<int>("SmtpPort"),
                         smtpSettings.GetValue<string>("SmtpPass"),
                         smtpSettings.GetValue<string>("SmtpUser")
                     );
                })
                .AddSingleton(new MongoHandler(
                    Configuration.GetConnectionString("MongoHost"),
                    isDev ? "" : Configuration.GetConnectionString("MongoUser"),
                    isDev ? "" : Configuration.GetConnectionString("MongoPwd"),
                    isDev ? "" : Configuration.GetConnectionString("MongoDb")
                    )
                );

            //codice aggiunto (insieme al pacchetto nuget AddRazorRuntimeCompilation) per permettere di vedere le modifiche ai file cshmtl senza ricompilare
            if (isDev)
            {
                services.AddControllersWithViews().AddRazorRuntimeCompilation();
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            /// DEFAULT USERS AND ROLES
            serviceProvider.GetService<ApplicationDbContext>().Database.EnsureCreated();

            Task<bool> roleCheck = roleManager.RoleExistsAsync("WebAdmin");

            if (!roleCheck.Result)
            {
                roleManager.CreateAsync(new IdentityRole("WebAdmin")).Wait();
            }

            Task<AppUser> userSearch = userManager.FindByNameAsync("Fabrizio");
            Task<IdentityResult> identityResult;
            if (userSearch.Result == null)
            {
                AppUser user = new AppUser("Fabrizio")
                {
                    Email = "fabrizio.durante@coservizi.it"
                };

                identityResult = userManager.CreateAsync(user, "Envr10nm3ntal#*");

                if (identityResult.Result.Succeeded)
                    userManager.AddToRoleAsync(user, "WebAdmin").Wait();
            }

            userSearch = userManager.FindByNameAsync("Andreino");
            if (userSearch.Result == null)
            {
                AppUser user = new AppUser("Andreino")
                {
                    Email = "durante@coservizi.it"
                };

                identityResult = userManager.CreateAsync(user, "Envr10nm3ntal#*");

                if (identityResult.Result.Succeeded)
                    userManager.AddToRoleAsync(user, "WebAdmin").Wait();
            }

            userSearch = userManager.FindByNameAsync("TestUsr");
            if (userSearch.Result == null)
            {
                AppUser user = new AppUser("TestUsr")
                {
                    Email = "test.env.coll@mailnesia.com"
                };

                userManager.CreateAsync(user, "TestEnv1!").Wait();
            }
            
            userSearch = userManager.FindByNameAsync("AvisAlassio");
            if (userSearch.Result == null)
            {
                AppUser user = new AppUser("AvisAlassio")
                {
                    Email = "avis.alassio@avis.it"
                };

                userManager.CreateAsync(user, "Alassio@2021").Wait();
            }
        }
    }
}
