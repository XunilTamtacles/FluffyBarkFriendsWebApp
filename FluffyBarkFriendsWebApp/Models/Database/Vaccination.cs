using System;
using System.Collections.Generic;

namespace FluffyBarkFriendsWebApp.Models.Database;

public partial class Vaccination
{
    public int VaccinationId { get; set; }

    public int PetId { get; set; }

    public int AppointmentId { get; set; }

    public string VaccineName { get; set; } = null!;

    public DateOnly DateGiven { get; set; }

    public DateOnly? NextDueDate { get; set; }

    public string? Dose { get; set; }

    public string? Remarks { get; set; }

    public int RecordedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsSent { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Pet Pet { get; set; } = null!;

    public virtual User RecordedByUser { get; set; } = null!;
}
