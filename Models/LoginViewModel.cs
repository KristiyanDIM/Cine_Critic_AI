using System.ComponentModel.DataAnnotations;

namespace Cine_Critic_AI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Моля, въведете имейл.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Моля, въведете парола.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомни ме")]
        public bool RememberMe { get; set; }
    }
}
