using Cine_Critic_AI.Models;

namespace Cine_Critic_AI.Services.Factories
{
    // Фабрика за създаване на ревю, въведено ръчно от потребителя
    public class ManualReviewFactory : IReviewFactory
    {
        // Създава обект Review на базата на ръчно подадени стойности
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
