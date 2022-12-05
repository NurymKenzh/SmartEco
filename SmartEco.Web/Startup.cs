using Microsoft.AspNetCore.Mvc.Razor;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SmartEco.Web.Services.Providers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartEco.Web.Helpers;
using SmartEco.Web.Helpers.Filters;
using SmartEco.Web.Services;
using SmartEco.Web.Extensions;

namespace SmartEco
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60); // Set a short timeout for easy testing.
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;  // Make the session cookie essential
            });

            services
                .AddControllersWithViews(opt => {
                    opt.Filters.Add(new WriteToSession());
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResources));
                })
                .AddRazorRuntimeCompilation();

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient<ISmartEcoApi, SmartEcoApi>(client =>
            {
                client.BaseAddress = new Uri(Configuration.GetValue<string>("SmartEcoApiUrl"));
                client.Timeout = TimeSpan.FromMinutes(5);

            });

            services.AddCustomServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }

            var supportedCultures = new[]
            {
                new CultureInfo("kk"),
                new CultureInfo("ru"),
                new CultureInfo("en")
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru"),
                SupportedCultures = supportedCultures,  // Formatting numbers, dates, etc.
                SupportedUICultures = supportedCultures // UI strings that we have localized.
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseSession();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllerRoute(
                    name: "default", 
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
