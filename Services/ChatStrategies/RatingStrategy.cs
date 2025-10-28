using System.Text.Json;

namespace Cine_Critic_AI.Services.ChatStrategies
{
    public class RatingStrategy : IChatStrategy
    {
        public async Task<string> GenerateResponseAsync(string userMessage, int userId, LocalAIService ai)
        {
            var json = JsonSerializer.Serialize(new
            {
                model = "llama3",
                prompt = $"Ти си AI филмов критик. Дай оценка или кратко ревю за: {userMessage}"
            });
            return await ai.PostToOllamaAsync(json);
        }
    }
}
