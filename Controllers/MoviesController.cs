using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Cine_Critic_AI.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AppLoggerSingleton _appLogger;

        public MoviesController(ApplicationDbContext context, AppLoggerSingleton appLogger)
        {
            _context = context;
            _appLogger = appLogger;
        }

        // ✅ малък помощен метод за име на текущия потребител
        private string GetCurrentUser()
        {
            return User.Identity != null && User.Identity.IsAuthenticated
                ? User.Identity.Name
                : "Анонимен потребител";
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            _appLogger.Log($"{GetCurrentUser()} зареди списъка с филми.");
            return View(movies);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да достъпи детайли на филм без ID.");
                return NotFound();
            }

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да достъпи несъществуващ филм (ID {id}).");
                return NotFound();
            }

            _appLogger.Log($"{GetCurrentUser()} прегледа детайлите на филма: {movie.Title}");
            return View(movie);
        }

        // GET: Movies/Create
        [Authorize]
        public IActionResult Create()
        {
            _appLogger.Log($"{GetCurrentUser()} отвори страницата за създаване на нов филм.");
            return View();
        }

        // POST: Movies/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year,Genre,Director,Description")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();

                _appLogger.Log($"{GetCurrentUser()} добави нов филм: {movie.Title} ({movie.Year})");
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да редактира филм без ID.");
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да редактира несъществуващ филм (ID {id}).");
                return NotFound();
            }

            _appLogger.Log($"{GetCurrentUser()} отвори страницата за редакция на филма: {movie.Title}");
            return View(movie);
        }

        // POST: Movies/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,Genre,Director,Description")] Movie movie)
        {
            if (id != movie.Id)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да редактира филм с несъответстващо ID ({id}).");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    _appLogger.Log($"{GetCurrentUser()} редактира филма: {movie.Title}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        _appLogger.Log($"{GetCurrentUser()} опита да редактира несъществуващ филм (ID {movie.Id}).");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да изтрие филм без ID.");
                return NotFound();
            }

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да изтрие несъществуващ филм (ID {id}).");
                return NotFound();
            }

            _appLogger.Log($"{GetCurrentUser()} отвори страницата за изтриване на филма: {movie.Title}");
            return View(movie);
        }

        // POST: Movies/Delete/5
        [Authorize]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                _appLogger.Log($"{GetCurrentUser()} изтри филма: {movie.Title}");
            }
            else
            {
                _appLogger.Log($"{GetCurrentUser()} опита да изтрие несъществуващ филм с ID {id}.");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id) => _context.Movies.Any(e => e.Id == id);
    }
}
