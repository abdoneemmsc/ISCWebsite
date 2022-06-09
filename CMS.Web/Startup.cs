// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using AutoMapper;
using DAL;
using DAL.Core;
using DAL.Core.Interfaces;
using DAL.Models;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CMS.Web.Authorization;
using CMS.Web.Helpers;
using System;
using System.Collections.Generic;
using AppPermissions = DAL.Core.ApplicationPermissions;
using System.Globalization;
using CMS.Web.Resources;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace CMS.Web
{
    public class Startup
    {
        private IWebHostEnvironment _env { get; }
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"], b => b.MigrationsAssembly("CMS.Web")));

            // add identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity options and password complexity here
            services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
            });
            services.AddMvc().AddRazorRuntimeCompilation();
            services.AddSession();
            services.AddDistributedMemoryCache();
            services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>();
            // Adds IdentityServer.
            //services.AddIdentityServer()
            //    // The AddDeveloperSigningCredential extension creates temporary key material for signing tokens.
            //    // This might be useful to get started, but needs to be replaced by some persistent key material for production scenarios.
            //    // See http://docs.identityserver.io/en/release/topics/crypto.html#refcrypto for more information.
            //    .AddDeveloperSigningCredential()
            //    .AddInMemoryPersistedGrants()
            //    // To configure IdentityServer to use EntityFramework (EF) as the storage mechanism for configuration data (rather than using the in-memory implementations),
            //    // see https://identityserver4.readthedocs.io/en/release/quickstarts/8_entity_framework.html
            //    .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
            //    .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
            //    .AddInMemoryClients(IdentityServerConfig.GetClients())
            //    .AddAspNetIdentity<ApplicationUser>()
            //    .AddProfileService<ProfileService>();


            var applicationUrl = Configuration["ApplicationUrl"].TrimEnd('/');

            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = applicationUrl;
            //        options.SupportedTokens = SupportedTokens.Jwt;
            //        options.RequireHttpsMetadata = false; // Note: Set to true in production
            //        options.ApiName = IdentityServerConfig.ApiName;
            //    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Authorization.Policies.ManageWebsitePolicy, policy => policy.RequireClaim(ClaimConstants.Permission, AppPermissions.ManageWebsite));
                options.AddPolicy(Authorization.Policies.ManageUsersPolicy, policy => policy.RequireClaim(ClaimConstants.Permission, AppPermissions.ManageUsers));
            });


            // Add cors
            services.AddCors();

            services.AddControllersWithViews();

            // In production, the Angular files will be served from this directory

            services.AddAutoMapper(typeof(Startup));

            // Configurations
            services.Configure<AppSettings>(Configuration);


            // Business Services
            services.AddScoped<IEmailSender, EmailSender>();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            // Repositories
            services.AddScoped<IUnitOfWork, HttpUnitOfWork>();
            services.AddScoped<IAccountManager, AccountManager>();
            services.ConfigureApplicationCookie(options => options.LoginPath = "/ar/cpanel/account/login");

            // Auth Handlers
            services.AddSingleton<IAuthorizationHandler, ViewUserAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ManageUserAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, ViewRoleAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AssignRolesAuthorizationHandler>();

            // DB Creation and Seeding
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMvc()
        .AddDataAnnotationsLocalization(options =>
        {
            options.DataAnnotationLocalizerProvider = (type, factory) =>
                factory.Create(typeof(Resource));
        });
            services.AddMvc()
              .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
              .AddDataAnnotationsLocalization();
            services.AddSingleton(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.Arabic }));
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var supportedCultures = new[]
              {
                      new CultureInfo("en-US"),
                      new CultureInfo("ar"),
                };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ar"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            Utilities.ConfigureLogger(loggerFactory);
            EmailTemplates.Initialize(env);

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

            app.UseRouting();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());

            //app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePagesWithRedirects("/Error/{0}");
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "InternationalizationAreasDefault",
                    areaName: "cpanel",
                    pattern: "{culture}/cpanel/{controller=Home}/{action=Index}/{id?}/{title?}"
                , defaults: new { culture = "ar", controller = "Home", action = "Index" }
                    );
                endpoints.MapAreaControllerRoute(
                    name: "AreasDefault",
                    areaName: "cpanel",
                    pattern: "cpanel/{controller=Home}/{action=Index}/{id?}/{title?}");

                endpoints.MapControllerRoute(
                name: "InternationalizationDefault",
                pattern: "{culture}/{controller}/{action}/{id?}/{title?}"
                , defaults: new { culture = "ar", controller = "Home", action = "Index" }
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}/{title?}");

            });

        }
    }
}
