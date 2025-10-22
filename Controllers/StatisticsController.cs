using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Cine_Critic_AI.Controllers
{
    [Authorize] // само логнати потребители да виждат статистиката
    public class StatisticsController : Controller
    {
        private readonly DatabaseService _db;

        public StatisticsController(DatabaseService db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var reviews = _db.GetAllReviews();
            var movies = _db.GetAllMovies();

            var stats = new StatisticsViewModel
            {
                TotalReviews = reviews.Count(),
                TotalMovies = movies.Count(),
                AverageRating = reviews.Any() ? reviews.Average(r => r.Rate) : 0,
                TopMovie = reviews
                    .GroupBy(r => r.MovieId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => movies.FirstOrDefault(m => m.Id == g.Key)?.Title ?? "Неизвестен")
                    .FirstOrDefault() ?? "Няма ревюта"
            };

            return View(stats);
        }
    }

   
}
