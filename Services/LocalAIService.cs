using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace Cine_Critic_AI.Services
{
    public class LocalAIService
    {
        private readonly HttpClient _http;

        public LocalAIService(HttpClient http)
        {
            _http = http;
            // Адресът на локалния Ollama сървър
            _http.BaseAddress = new Uri("http://127.0.0.1:11434/");
        }

        /// <summary>
        /// Генерира ревю за филм с оценка и емоционален тон.
        /// </summary>
        public async Task<string> GenerateReviewAsync(string title, string description)
        {
            var json = JsonSerializer.Serialize(new
            {
                model = "llama3",
                prompt = $"Напиши кратко и смислено ревю за филм. Заглавие: '{title}'. Описание: {description}. " +
                         "Добави мнение в няколко изречения, включи оценка (1-5) и емоционален тон (позитивен, неутрален или негативен)."
            });

            var response = await _http.PostAsync("api/generate", new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return ExtractResponseText(content);
        }

        /// <summary>
        /// Анализира текста и връща неговия емоционален тон.
        /// </summary>
        public async Task<string> ExtractEmotionFromTextAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "неутрален";

            var json = JsonSerializer.Serialize(new
            {
                model = "llama3",
                prompt = $"Определи емоционалния тон на следния текст като една дума (позитивен, неутрален или негативен): {text}"
            });

            try
            {
                var response = await _http.PostAsync("api/generate", new StringContent(json, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // Ollama връща JSON редове — комбинираме ги
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var sb = new StringBuilder();
                foreach (var line in lines)
                {
                    var doc = JsonDocument.Parse(line);
                    if (doc.RootElement.TryGetProperty("response", out var resp))
                        sb.Append(resp.GetString());
                }

                var result = sb.ToString().Trim().ToLower();

                // Опростена обработка на отговора
                if (result.Contains("позитив")) return "позитивен";
                if (result.Contains("негатив")) return "негативен";
                return "неутрален";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ AI грешка: {ex.Message}");
                return "грешка при анализ";
            }
        }


        /// <summary>
        /// Извлича текстовия отговор от Ollama streaming формат.
        /// </summary>
        private static string ExtractResponseText(string content)
        {
            var sb = new StringBuilder();
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                try
                {
                    using var doc = JsonDocument.Parse(line);
                    if (doc.RootElement.TryGetProperty("response", out var resp))
                        sb.Append(resp.GetString());
                }
                catch
                {
                    // игнорирай грешки от невалиден JSON ред
                }
            }

            return sb.ToString().Trim();
        }
    }
}
