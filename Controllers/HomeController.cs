using Cine_Critic_AI.Services;
using CineCritic_AI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CineCritic_AI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // Singleton Logger за цялото приложение
        private readonly AppLoggerSingleton _appLogger;

        // Конструктор с Dependency Injection
        public HomeController(ILogger<HomeController> logger, AppLoggerSingleton appLogger)
        {
            _logger = logger;
            _appLogger = appLogger;
        }

        public IActionResult Index()
        {
            //Логваме посещението на началната страница
            _appLogger.Log("Потребителят е посетил началната страница.");
            return View();
        }
        public IActionResult Logs()
        {
            var logs = _appLogger.GetLogs(); // Вземаме всички записани логове
            return View(logs); // Предаваме ги на View
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
