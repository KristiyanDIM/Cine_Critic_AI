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
        [Display(Name = "Оценка")]
        public int Rate { get; set; }

        [Display(Name = "Коментар (по избор)")]
        public string? Comment { get; set; } // вече не е задължително

        [StringLength(50)]
        [Display(Name = "Емоционален тон")]
        public string? EmotionTone { get; set; } // може и това да е незадължително

        [Required]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; } = DateTime.Now;

        public int UserId { get; set; } // Връзка към потребителя
        public int MovieId { get; set; } // Връзка към филма
    }
}
