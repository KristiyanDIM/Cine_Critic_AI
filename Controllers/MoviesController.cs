using Microsoft.AspNetCore.Mvc;
using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Authorization;

namespace Cine_Critic_AI.Controllers
{
    public class MoviesController : Controller
    {
        private readonly DatabaseService _database;
        private readonly AppLoggerSingleton _appLogger;

        public MoviesController(DatabaseService database, AppLoggerSingleton appLogger)
        {
            _database = database;
            _appLogger = appLogger;
        }

        private string GetCurrentUser()
        {
            return User.Identity != null && User.Identity.IsAuthenticated
                ? User.Identity.Name
                : "Анонимен потребител";
        }

        // GET: Movies
        public IActionResult Index()
        {
            var movies = _database.GetAllMovies();
            _appLogger.Log($"{GetCurrentUser()} зареди списъка с филми.");
            return View(movies);
        }

        // GET: Movies/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да достъпи детайли на филм без ID.");
                return NotFound();
            }

            var movie = _database.GetMovieById(id.Value);
            if (movie == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да достъпи несъществуващ филм (ID {id}).");
                return NotFound();
            }

            _appLogger.Log($"{GetCurrentUser()} прегледа детайлите на филма: {movie.Title}");
            return View(movie);
        }

        [Authorize]
        public IActionResult Create()
        {
            _appLogger.Log($"{GetCurrentUser()} отвори страницата за създаване на нов филм.");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _database.InsertMovie(movie);
                _appLogger.Log($"{GetCurrentUser()} добави нов филм: {movie.Title} ({movie.Year})");
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да редактира филм без ID.");
                return NotFound();
            }

            var movie = _database.GetMovieById(id.Value);
            if (movie == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да редактира несъществуващ филм (ID {id}).");
                return NotFound();
            }

            _appLogger.Log($"{GetCurrentUser()} отвори страницата за редакция на филма: {movie.Title}");
            return View(movie);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да редактира филм с несъответстващо ID ({id}).");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _database.UpdateMovie(movie);
                _appLogger.Log($"{GetCurrentUser()} редактира филма: {movie.Title}");
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да изтрие филм без ID.");
                return NotFound();
            }

            var movie = _database.GetMovieById(id.Value);
            if (movie == null)
            {
                _appLogger.Log($"{GetCurrentUser()} опита да изтрие несъществуващ филм (ID {id}).");
                return NotFound();
            }

            _appLogger.Log($"{GetCurrentUser()} отвори страницата за изтриване на филма: {movie.Title}");
            return View(movie);
        }

        [Authorize]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie = _database.GetMovieById(id);
            if (movie != null)
            {
                _database.DeleteMovie(id);
                _appLogger.Log($"{GetCurrentUser()} изтри филма: {movie.Title}");
            }
            else
            {
                _appLogger.Log($"{GetCurrentUser()} опита да изтрие несъществуващ филм с ID {id}.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
