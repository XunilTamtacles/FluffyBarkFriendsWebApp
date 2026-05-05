using System.ComponentModel.DataAnnotations;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class MedicalHistoryFormsViewModel
    {
        public int MedicalHistoryId { get; set; }

        [Required(ErrorMessage = "Pet is required.")]
        [Display(Name = "Pet")]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Visit date is required.")]
        [Display(Name = "Visit Date")]
        public DateOnly VisitDate { get; set; }

        [Display(Name = "Visit Time")]
        public TimeOnly? VisitTime { get; set; }

        [Required(ErrorMessage = "Condition is required.")]
        [StringLength(100)]
        public string Condition { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Diagnosis { get; set; }

        [StringLength(100)]
        public string? Treatment { get; set; }

        [StringLength(100)]
        public string? Dosage { get; set; }


        [StringLength(100)]
        public string? Medication { get; set; }

        [Required(ErrorMessage = "Created by user is required.")]
        [Display(Name = "Created By User")]
        public int CreatedByUserId { get; set; }
        public string Notes { get; internal set; }
    }
}
