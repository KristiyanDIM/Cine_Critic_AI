using Cine_Critic_AI.Models;

namespace Cine_Critic_AI.Services.Factories
{
    // Интерфейс за фабричен метод за създаване на ревю
    public interface IReviewFactory
    {
        // Създава ново ревю с подадени оценка, коментар и емоционален тон
        Review CreateReview(int rate, string comment, string emotion);
    }
}

