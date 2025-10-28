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
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.ExpireTimeSpan = TimeSpan.FromSeconds(5); // излиза от профила след 5 секунди след затварянето на браъзера
            options.SlidingExpiration = false;
            options.Cookie.IsEssential = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(5); // излиза от профила след 5 секунди след затварянето на браъзера
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDistributedMemoryCache();
            

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
