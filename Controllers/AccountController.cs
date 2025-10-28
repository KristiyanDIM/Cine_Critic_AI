using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cine_Critic_AI.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseService _database;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly AppLoggerSingleton _appLogger;

        public AccountController(DatabaseService database, AppLoggerSingleton appLogger)
        {
            _database = database;
            _passwordHasher = new PasswordHasher<User>();
            _appLogger = appLogger;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _database.GetAllUsers()
                                .FirstOrDefault(u => u.Username == model.Username);

            if (user == null ||
                _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Невалидно потребителско име или парола.");
                return View(model);
            }

            _appLogger.Log($"Потребителят {user.Username} се логна успешно.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                    new ClaimsPrincipal(claimsIdentity), authProperties).Wait();

            // ⚡ Добавяме UserId и Username в Session за ChatBot
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditProfile()
        {
            var user = _database.GetAllUsers()
                                .FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null) return NotFound();

            return View(new EditProfileViewModel { Username = user.Username });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            var user = _database.GetAllUsers()
                                .FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(model.Username) && user.Username != model.Username)
                user.Username = model.Username;

            if (!string.IsNullOrEmpty(model.NewPassword))
                user.Password = _passwordHasher.HashPassword(user, model.NewPassword);

            _database.UpdateUser(user);

            _appLogger.Log($"Потребителят {user.Username} е обновил профила си.");

            // Обновяване на claims и session
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme))).Wait();

            HttpContext.Session.SetString("Username", user.Username);

            TempData["Success"] = "Профилът е успешно обновен!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            var username = User.Identity.Name;
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

            // Изчистване на Session
            HttpContext.Session.Clear();

            _appLogger.Log($"Потребителят {username} се е излогнал.");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_database.GetAllUsers().Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Това потребителско име вече съществува.");
                return View(model);
            }

            if (_database.GetAllUsers().Any(u => u.Email == model.Email))
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

            _database.InsertUser(user);

            _appLogger.Log($"Ново регистриран потребител: {user.Username}");

            TempData["Success"] = "Регистрацията е успешна! Влез в профила си.";
            return RedirectToAction("Login");
        }
    }
}
