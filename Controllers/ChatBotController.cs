using Microsoft.AspNetCore.Mvc;

namespace Cine_Critic_AI.Controllers
{
    public class ChatBotController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        //за разработване
        [HttpPost]
        public IActionResult Ask(string userMessage)
        {
            string botResponse = $"🤖 AI: Благодаря за въпроса — '{userMessage}'. Скоро ще мога да отговоря по-интелигентно!";

            return Json(new { response = botResponse });
        }
    }
}
