using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class VaccinationRecordFormsViewModel
    {
        public int VaccinationId { get; set; }

        [Required(ErrorMessage = "Pet is required.")]
        [Display(Name = "Pet")]
        public int PetId { get; set; }

        [Display(Name = "Appointment")]
        public int? AppointmentId { get; set; }

        [Required(ErrorMessage = "Vaccine name is required.")]
        [Display(Name = "Vaccine Name")]
        public string VaccineName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date given is required.")]
        [Display(Name = "Date Given")]
        public DateOnly DateGiven { get; set; }

        [Display(Name = "Next Due Date")]
        public DateOnly? NextDueDate { get; set; }

        public string? Dose { get; set; }

        public string? Remarks { get; set; }

        [Required(ErrorMessage = "Recorded by user is required.")]
        [Display(Name = "Recorded By")]
        public int RecordedByUserId { get; set; }

        public List<SelectListItem> PetOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> AppointmentOptions { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> UserOptions { get; set; } = new List<SelectListItem>();
    }
}