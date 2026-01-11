using System.ComponentModel.DataAnnotations;

namespace Models.Request._user
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmNewPassword { get; set; }
    }
}
