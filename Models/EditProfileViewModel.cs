using System.ComponentModel.DataAnnotations;

namespace Cine_Critic_AI.Models
{
    public class EditProfileViewModel
    {
        [StringLength(50)]
        [Display(Name = "Потребителско име")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Нова парола")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Потвърди новата парола")]
        public string ConfirmPassword { get; set; }
    }
}
