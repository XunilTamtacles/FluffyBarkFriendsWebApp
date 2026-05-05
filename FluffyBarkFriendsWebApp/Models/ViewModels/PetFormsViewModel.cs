using System.ComponentModel.DataAnnotations;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class PetFormsViewModel
    {
        public int PetId { get; set; }

        [Required(ErrorMessage = "Pet name is required.")]
        [StringLength(30)]
        [Display(Name = "Pet Name")]
        public string PetName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Species is required.")]
        [StringLength(50)]
        public string Species { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Breed { get; set; }

        [Required(ErrorMessage = "Sex is required.")]
        [StringLength(30)]
        public string Sex { get; set; } = string.Empty;

        [Display(Name = "Birth Date")]
        public DateOnly? BirthDate { get; set; }

        [Range(0, 100, ErrorMessage = "Age must be 0 or greater.")]
        [Display(Name = "Age")]
        public int? AgeYears { get; set; }

        [StringLength(30)]
        public string? Color { get; set; }

        [Range(0, 999.99, ErrorMessage = "Weight must be 0 or greater.")]
        public decimal? Weight { get; set; }
    }
}
