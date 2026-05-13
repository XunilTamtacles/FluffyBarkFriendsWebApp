using System;
using System.Collections.Generic;

namespace FluffyBarkFriendsWebApp.Models.Database;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PetId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public string? ReasonVisit { get; set; }

    public string Status { get; set; } = null!;

    public string? Remarks { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Pet Pet { get; set; } = null!;

    public virtual ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
}
