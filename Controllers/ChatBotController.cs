using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Cine_Critic_AI.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly LocalAIService _ai;

        public ChatBotController(LocalAIService ai)
        {
            _ai = ai;
        }


        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return Json(new { response = "Моля, попитай нещо за филми 🎬" });

            try
            {
                var json = JsonSerializer.Serialize(new
                {
                    model = "llama3",
                    prompt = $"Ти си AI филмов критик. Отговаряй само на въпроси за филми, актьори, режисьори или ревюта. " +
                             $"Ако въпросът не е за кино, кажи 'Говоря само за кино.'\n\nПотребител: {userMessage}"
                });

                Console.WriteLine("📨 Изпратен prompt към Ollama:");
                Console.WriteLine(json);

                var response = await _ai.PostToOllamaAsync(json);

                Console.WriteLine("🎬 Отговор от Ollama:");
                Console.WriteLine(response);

                return Json(new { response });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Chat AI error: {ex.Message}");
                return Json(new { response = "⚠️ Грешка при свързване с AI." });
            }
        }




    }
}
