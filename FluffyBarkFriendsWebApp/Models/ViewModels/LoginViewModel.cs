using System.ComponentModel.DataAnnotations;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "UserName is required.")]
        [Display(Name = "UserName")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}