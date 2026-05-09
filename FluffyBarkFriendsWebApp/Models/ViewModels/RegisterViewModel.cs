using System.ComponentModel.DataAnnotations;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserName is required.")]
        [Display(Name = "UserName")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } = "Client";
    }
}