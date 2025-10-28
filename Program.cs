using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;

namespace CineCritic_AI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // Регистрираме Singleton Database (DAO)
            builder.Services.AddSingleton<DatabaseService>(DatabaseService.Instance);

            // Регистрираме Logger Singleton
            builder.Services.AddSingleton(AppLoggerSingleton.Instance);
            builder.Services.AddHttpClient<LocalAIService>();

            // Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login"; // къде да се пренасочи, ако не е логнат
                    options.LogoutPath = "/Account/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // изход след 30 минути бездействие
                    options.SlidingExpiration = true; // удължава живота на cookie при активност
                });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // излиза от профила след 30 минути бездействие
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Middleware за Session и Authentication
            app.UseStaticFiles();
            app.UseRouting();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
