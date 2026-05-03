using System;
using System.Collections.Generic;

namespace FluffyBarkFriendsWebApp.Models.Database;

public partial class MedicalHistory
{
    public int MedicalHistoryId { get; set; }

    public int PetId { get; set; }

    public DateOnly VisitDate { get; set; }

    public TimeOnly? VisitTime { get; set; }

    public string Condition { get; set; } = null!;

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string? Dosage { get; set; }

    public string? Notes { get; set; }

    public string? Medication { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Pet Pet { get; set; } = null!;

    public virtual Appointment? Appointment { get; set; }

    public virtual User RecordedByUser { get; set; } = null!;
}
