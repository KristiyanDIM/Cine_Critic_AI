namespace Cine_Critic_AI.Models
{
    public class StatisticsViewModel
    {
        public int TotalReviews { get; set; }
        public int TotalMovies { get; set; }
        public double AverageRating { get; set; }
        public string TopMovie { get; set; } = "";
    }
}
