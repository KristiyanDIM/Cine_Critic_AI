using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cine_Critic_AI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly AppLoggerSingleton _appLogger;

        public AccountController(ApplicationDbContext context, AppLoggerSingleton appLogger)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _appLogger = appLogger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Невалидно потребителско име или парола.");
                return View(model);
            }

            // Логваме успешен вход
            _appLogger.Log($"Потребителят {user.Username} се логна успешно.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null) return NotFound();

            return View(new EditProfileViewModel { Username = user.Username });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(model.Username) && user.Username != model.Username)
                user.Username = model.Username;

            if (!string.IsNullOrEmpty(model.NewPassword))
                user.Password = _passwordHasher.HashPassword(user, model.NewPassword);

            _context.Update(user);
            await _context.SaveChangesAsync();

            // Логваме промяна на профил
            _appLogger.Log($"Потребителят {user.Username} е обновил профила си.");

            // Преавтентикация
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));

            TempData["Success"] = "Профилът е успешно обновен!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Логваме изход
            _appLogger.Log($"Потребителят {username} се е излогнал.");

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Това потребителско име вече съществува.");
                return View(model);
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Този имейл вече е регистриран.");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                RegisteredOn = DateTime.Now,
                Password = _passwordHasher.HashPassword(null, model.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Логваме регистрация
            _appLogger.Log($"Ново регистриран потребител: {user.Username}");

            TempData["Success"] = "Регистрацията е успешна! Влез в профила си.";
            return RedirectToAction("Login");
        }
    }
}
