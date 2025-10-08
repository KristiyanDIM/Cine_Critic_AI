using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cine_Critic_AI.Models;
using Microsoft.AspNetCore.Identity;

namespace Cine_Critic_AI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Невалидно потребителско име или парола.");
                return View(model);
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Невалидно потребителско име или парола.");
                return View(model);
            }

            TempData["Welcome"] = $"Добре дошъл, {user.Username}!";
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

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
                RegisteredOn = DateTime.Now
            };

            // Хеширане на паролата
            user.Password = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Регистрацията е успешна! Влез в профила си.";
            return RedirectToAction("Login");
        }
    }
}
