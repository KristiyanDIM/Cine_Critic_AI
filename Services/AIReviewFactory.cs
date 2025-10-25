using Cine_Critic_AI.Models;

namespace Cine_Critic_AI.Services.Factories
{
    // Фабрика за създаване на ревю, генерирано с помощта на AI
    public class AIReviewFactory : IReviewFactory
    {
        private readonly LocalAIService _ai;

        // Конструктор, който получава AI услугата чрез dependency injection
        public AIReviewFactory(LocalAIService ai)
        {
            _ai = ai;
        }

        // Създава обект Review с използване на данни, подадени от AI
        public Review CreateReview(int rate, string comment, string emotion)
        {
            return new Review
            {
                Rate = rate,
                Comment = comment,
                EmotionTone = string.IsNullOrEmpty(emotion) ? "неутрален" : emotion,
                Date = DateTime.Now
            };
        }
    }
}
