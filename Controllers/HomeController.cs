using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using CineCritic_AI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;


namespace CineCritic_AI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppLoggerSingleton _appLogger;
        private readonly DatabaseService _database; // добавяме базата

        public HomeController(ILogger<HomeController> logger, AppLoggerSingleton appLogger, DatabaseService database)
        {
            _logger = logger;
            _appLogger = appLogger;
            _database = database;
        }

        public IActionResult Index()
        {
            // 🧾 Логваме посещението на началната страница
            _appLogger.Log("Потребителят е посетил началната страница.");

            var allMovies = _database.GetAllMovies();
            var allReviews = _database.GetAllReviews();

            var random = new Random();
            var randomMovies = allMovies.OrderBy(x => random.Next()).Take(3).ToList();
            var randomReviews = allReviews.OrderBy(x => random.Next()).Take(3).ToList();

            var stats = new StatisticsViewModel
            {
                TotalMovies = allMovies.Count,
                TotalReviews = allReviews.Count,
                AverageRating = allReviews.Any() ? allReviews.Average(r => r.Rate) : 0,
                TopMovie = allMovies
                    .OrderByDescending(m => allReviews.Count(r => r.MovieId == m.Id))
                    .FirstOrDefault()?.Title ?? "Няма данни"
            };

            var model = new HomePageViewModel
            {
                RandomMovies = randomMovies,
                RandomReviews = randomReviews,
                Statistics = stats
            };

            return View(model);
        }

        public IActionResult Logs()
        {
            var logs = _appLogger.GetLogs();
            return View(logs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

   

    
}
