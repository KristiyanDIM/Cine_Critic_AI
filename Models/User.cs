using System.ComponentModel.DataAnnotations;

namespace Cine_Critic_AI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Имейл")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; }

        [Display(Name = "Дата на регистрация")]
        public DateTime RegisteredOn { get; set; } = DateTime.Now;
    }
}

