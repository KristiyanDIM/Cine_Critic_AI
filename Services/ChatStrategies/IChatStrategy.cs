namespace Cine_Critic_AI.Services.ChatStrategies
{
    public interface IChatStrategy
    {
        Task<string> GenerateResponseAsync(string userMessage, int userId, LocalAIService ai);
    }

}
