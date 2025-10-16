using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cine_Critic_AI.Services
{
    public class LocalAIService
    {
        private readonly HttpClient _http;

        public LocalAIService()
        {
            _http = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:11434/") };
        }

        public async Task<string> GenerateReviewAsync(string title, string description)
        {
            var json = JsonSerializer.Serialize(new
            {
                model = "llama3",
                prompt = $"Напиши кратко ревю за филм '{title}'. {description} Включи оценка (1-5) и емоционален тон."
            });

            var response = await _http.PostAsync("api/generate", new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            // Обединяване на streaming output, ако има такъв
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                var doc = JsonDocument.Parse(line);
                if (doc.RootElement.TryGetProperty("response", out var resp))
                {
                    sb.Append(resp.GetString());
                }
            }

            return sb.ToString();
        }
    }
}
