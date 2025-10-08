using Acr.UserDialogs;
using Cine_Critic_AI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cine_Critic_AI.Controllers
{
    public class AccountControllerRegister : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountControllerRegister(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Невалидно потребителско име или парола.");
                return View(model);
            }

            TempData["Welcome"] = $"Добре дошъл, {user.Username}!";
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
            if (!ModelState.IsValid)
                return View(model);

            // Проверка дали потребителското име или имейл вече съществува
            if (await _context.Set<User>().AnyAsync(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Това потребителско име вече съществува.");
                return View(model);
            }

            if (await _context.Set<User>().AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Този имейл вече е регистриран.");
                return View(model);
            }

            // Създай нов потребител
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password, // ⚠️ В реален проект трябва да се криптира
                RegisteredOn = DateTime.Now
            };

            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Регистрацията е успешна! Влез в профила си.";
            return RedirectToAction("Login");
        }
    }
}
