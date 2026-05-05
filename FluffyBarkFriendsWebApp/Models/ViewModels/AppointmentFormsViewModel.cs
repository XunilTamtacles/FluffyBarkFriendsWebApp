using System.ComponentModel.DataAnnotations;

namespace FluffyBarkFriendsWebApp.Models.ViewModels
{
    public class AppointmentFormsViewModel
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Pet is required.")]
        [Display(Name = "Pet")]
        public int PetId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        [Display(Name = "Appointment Date")]
        public DateOnly AppointmentDate { get; set; }

        [Required(ErrorMessage = "Appointment time is required.")]
        [Display(Name = "Appointment Time")]
        public TimeOnly AppointmentTime { get; set; }

        [Display(Name = "Reason for Visit")]
        [StringLength(50)]
        public string? ReasonVisit { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(100)]
        public string Status { get; set; } = "Pending";

        [StringLength(255)]
        public string? Remarks { get; set; }

        [Required(ErrorMessage = "Created by user is required.")]
        [Display(Name = "Created By User")]
        public int CreatedByUserId { get; set; }
    }
}
