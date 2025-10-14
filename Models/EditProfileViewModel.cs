using System.ComponentModel.DataAnnotations;

namespace Cine_Critic_AI.Models
{
    public class EditProfileViewModel
    {
        [StringLength(50)]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; }  // НЕ е Required

        [DataType(DataType.Password)]
        [Display(Name = "Нова парола")]
        public string NewPassword { get; set; } // НЕ е Required

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Паролите не съвпадат.")]
        [Display(Name = "Потвърди новата парола")]
        public string ConfirmPassword { get; set; } // НЕ е Required
    }
}
