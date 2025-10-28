namespace Cine_Critic_AI.Services.ChatStrategies
{
    public class ChatContext
    {
        private IChatStrategy _strategy;

        public void SetStrategy(IChatStrategy strategy)
        {
            _strategy = strategy;
        }

        public async Task<string> ExecuteStrategy(string userMessage, int userId, LocalAIService ai)
        {
            if (_strategy == null)
                throw new InvalidOperationException("Strategy not set!");
            return await _strategy.GenerateResponseAsync(userMessage, userId, ai);
        }
    }
}
