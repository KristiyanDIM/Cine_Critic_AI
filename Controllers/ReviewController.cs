using Microsoft.AspNetCore.Mvc;
using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Authorization;

namespace Cine_Critic_AI.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly DatabaseService _database;
        private readonly AppLoggerSingleton _appLogger;

        public ReviewsController(DatabaseService database, AppLoggerSingleton appLogger)
        {
            _database = database;
            _appLogger = appLogger;
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
    }
}
