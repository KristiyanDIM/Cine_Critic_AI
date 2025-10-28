﻿using Cine_Critic_AI.Models;
using Cine_Critic_AI.Services;
using Cine_Critic_AI.Services.ChatStrategies;
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

            var userChatMessage = new ChatMessage
            {
                UserId = userId,
                Sender = "User",
                Message = userMessage.Trim(),
                Timestamp = DateTime.Now
            };
            DatabaseService.Instance.InsertChatMessage(userChatMessage);

            // 🔹 Избор на стратегия
            var chatContext = new ChatContext();
            if (userMessage.Contains("препоръчвай", StringComparison.OrdinalIgnoreCase))
                chatContext.SetStrategy(new RecommendationStrategy());
            else if (userMessage.Contains("оцени", StringComparison.OrdinalIgnoreCase))
                chatContext.SetStrategy(new RatingStrategy());
            else
                chatContext.SetStrategy(new AnalysisStrategy());

            // 🔹 Изпълнение на стратегия
            string responseText = await chatContext.ExecuteStrategy(userMessage, userId, _ai);
            responseText = string.IsNullOrWhiteSpace(responseText) ? "⚠️ AI не върна отговор." : responseText;

            var botChatMessage = new ChatMessage
            {
                UserId = userId,
                Sender = "Bot",
                Message = responseText,
                Timestamp = DateTime.Now
            };
            DatabaseService.Instance.InsertChatMessage(botChatMessage);

            return Json(new { response = responseText });
        }


        [HttpPost]
        public IActionResult ClearChat([FromForm] int userId)
        {
            DatabaseService.Instance.ClearChatByUser(userId);
            return Ok();
        }




    }
}
