using System.ComponentModel.DataAnnotations;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class PetFormsViewModel
    {
        public int PetId { get; set; }

        [Required]
        public string PetName { get; set; } = string.Empty;

        [Required]
        public string Species { get; set; } = string.Empty;

        public string? Breed { get; set; }

        [Required]
        public string Sex { get; set; } = string.Empty;

        public DateOnly? BirthDate { get; set; }

        public int? AgeYears { get; set; }

        public string? Color { get; set; }

        public decimal? Weight { get; set; }

        [Display(Name = "Owner Name")]
        public string? OwnerName { get; set; }

        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        public string? Notes { get; set; }
    }
}