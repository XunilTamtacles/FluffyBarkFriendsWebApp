using System.ComponentModel.DataAnnotations;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class UserFormsViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(30)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserName is required.")]
        [StringLength(30)]
        [Display(Name = "UserName")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        [StringLength(30)]
        public string Role { get; set; } = string.Empty;
    }
}
