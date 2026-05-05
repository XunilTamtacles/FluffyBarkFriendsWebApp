using System.ComponentModel.DataAnnotations;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class VaccinationRecordFormsViewModel
    {
        public int VaccinationId { get; set; }

        [Required(ErrorMessage = "Pet is required.")]
        [Display(Name = "Pet")]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Appointment is required.")]
        [Display(Name = "Appointment")]
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Vaccine name is required.")]
        [StringLength(100)]
        [Display(Name = "Vaccine Name")]
        public string VaccineName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date given is required.")]
        [Display(Name = "Date Given")]
        [DataType(DataType.Date)]
        public DateOnly DateGiven { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Display(Name = "Next Due Date")]
        [DataType(DataType.Date)]
        public DateOnly? NextDueDate { get; set; }

        [StringLength(30)]
        public string? Dose { get; set; }

        [StringLength(100)]
        public string? Remarks { get; set; }

        [Required(ErrorMessage = "Recorded by user is required.")]
        [Display(Name = "Recorded By User")]
        public int RecordedByUserId { get; set; }
    }
}