using System.ComponentModel.DataAnnotations;

namespace Cine_Critic_AI.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Моля, въведете потребителско име.")]
        [StringLength(50)]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Моля, въведете имейл.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Моля, въведете парола.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Паролата трябва да е поне 6 символа.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Моля, потвърдете паролата.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        [Display(Name = "Потвърди паролата")]
        public string ConfirmPassword { get; set; }
    }
}
