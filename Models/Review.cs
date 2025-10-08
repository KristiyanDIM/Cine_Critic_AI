using System;
using System.ComponentModel.DataAnnotations;

namespace Cine_Critic_AI.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Оценката трябва да бъде между 1 и 5.")]
        public int Rate { get; set; } // Rating out of 5
        public string Comment { get; set; }
        [StringLength(50)]
        [Display(Name = "Емоционален тон")]
        public string EmotionTone { get; set; } // например: "позитивен", "неутрален", "негативен"
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
