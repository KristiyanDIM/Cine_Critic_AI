using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Cine_Critic_AI.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly DatabaseService _database;
        private readonly AppLoggerSingleton _appLogger;
        private readonly LocalAIService _ai;

        // Единен конструктор – съдържа всички зависимости
        public ReviewsController(DatabaseService database, AppLoggerSingleton appLogger, LocalAIService ai)
        {
            _database = database;
            _appLogger = appLogger;
            _ai = ai;
        }

        // GET: Reviews
        public IActionResult Index()
        {
            var reviews = _database.GetAllReviews();
            return View(reviews);
        }

        // GET: Reviews/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var review = _database.GetReviewById(id.Value);
            if (review == null)
                return NotFound();

            return View(review);
        }

        // GET: Reviews/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reviews/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Review review)
        {
            if (ModelState.IsValid)
            {
                _database.InsertReview(review);
                _appLogger.Log($"Потребителят създаде ново ревю (ID {review.Id}).");
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var review = _database.GetReviewById(id.Value);
            if (review == null)
                return NotFound();

            return View(review);
        }

        // POST: Reviews/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Review review)
        {
            if (id != review.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _database.UpdateReview(review);
                _appLogger.Log($"Потребителят редактира ревюто (ID {review.Id}).");
                return RedirectToAction(nameof(Index));
            }
            return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var review = _database.GetReviewById(id.Value);
            if (review == null)
                return NotFound();

            return View(review);
        }

        // POST: Reviews/DeleteConfirmed/5
        [Authorize]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var review = _database.GetReviewById(id);
            if (review != null)
            {
                _database.DeleteReview(id);
                _appLogger.Log($"Потребителят изтри ревюто (ID {id}).");
            }

            return RedirectToAction(nameof(Index));
        }

        // AI функционалност
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Generate(string description)
        {
            var generatedText = await _ai.GenerateReviewAsync("AI Generated Review", description);
            var emotion = await _ai.ExtractEmotionFromTextAsync(generatedText);
            var rating = ExtractRatingFromText(generatedText ?? "");

            return Json(new
            {
                comment = generatedText ?? "",
                emotion = emotion ?? "неутрален",
                rate = rating > 0 ? rating : 3
            });
        }



        private int ExtractRatingFromText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;

            var match = Regex.Match(text, @"([1-5])\s*(\/\s*5|от\s*5|рейтинг|rating|оценка)?", RegexOptions.IgnoreCase);
            return match.Success && int.TryParse(match.Groups[1].Value, out int value) ? value : 0;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AnalyzeEmotion([FromForm] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Content("неутрален");

            try
            {
                var emotion = await _ai.ExtractEmotionFromTextAsync(text);
                if (string.IsNullOrWhiteSpace(emotion))
                    emotion = "неутрален";

                // върни само чист текст (по-лесно за JS)
                return Content(emotion);
            }
            catch (Exception ex)
            {
                // логни грешката
                _appLogger.Log($"AnalyzeEmotion error: {ex.Message}");
                return Content("грешка при анализ");
            }
        }







    }
}
