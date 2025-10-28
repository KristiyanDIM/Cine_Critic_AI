using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Cine_Critic_AI.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly LocalAIService _ai;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatBotController(LocalAIService ai, IHttpContextAccessor httpContextAccessor)
        {
            _ai = ai;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        public IActionResult Index()
        {
            // Вземаме userId от сесия
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            List<ChatMessage> messages = new List<ChatMessage>();

            if (userId > 0)
            {
                // Взимаме съобщенията на потребителя
                messages = DatabaseService.Instance.GetChatMessagesByUser(userId);
            }

            return View(messages);
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] string userMessage, [FromForm] int userId)
        {
            if (userId <= 0)
                return Json(new { response = "⚠️ Не е намерен валиден потребител. Моля, влезте в системата." });

            if (string.IsNullOrWhiteSpace(userMessage))
                return Json(new { response = "Моля, попитай нещо за филми 🎬" });

            try
            {
                // 1️⃣ Записваме съобщението на потребителя
                var userChatMessage = new ChatMessage
                {
                    UserId = userId,
                    Sender = "User",
                    Message = userMessage.Trim(),
                    Timestamp = DateTime.Now
                };
                DatabaseService.Instance.InsertChatMessage(userChatMessage);

                // 2️⃣ Подготвяме prompt за AI
                var json = JsonSerializer.Serialize(new
                {
                    model = "llama3",
                    prompt = $"Ти си AI филмов критик. Отговаряй само на въпроси за филми, актьори, режисьори или ревюта. " +
                             $"Ако въпросът не е за кино, кажи 'Говоря само за кино.'\n\nПотребител: {userMessage}"
                });

                // 3️⃣ Изпращаме към AI
                var responseText = await _ai.PostToOllamaAsync(json);
                responseText = string.IsNullOrWhiteSpace(responseText)
                    ? "⚠️ AI не върна отговор."
                    : responseText;

                // 4️⃣ Записваме отговора на бота
                var botChatMessage = new ChatMessage
                {
                    UserId = userId,
                    Sender = "Bot",
                    Message = responseText,
                    Timestamp = DateTime.Now
                };
                DatabaseService.Instance.InsertChatMessage(botChatMessage);

                // 5️⃣ Връщаме отговора като JSON
                return Json(new { response = responseText });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Chat AI error: {ex.Message}");
                return Json(new { response = "⚠️ Грешка при свързване с AI." });
            }
        }


        [HttpPost]
        public IActionResult ClearChat([FromForm] int userId)
        {
            DatabaseService.Instance.ClearChatByUser(userId);
            return Ok();
        }




    }
}
