namespace Cine_Critic_AI.Models
{
    public class HomePageViewModel
    {
        public List<Movie> RandomMovies { get; set; } = new();
        public List<Review> RandomReviews { get; set; } = new();
        public StatisticsViewModel Statistics { get; set; } = new();
    }
}
