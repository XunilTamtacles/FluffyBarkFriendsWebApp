using System;
using System.Collections.Generic;

namespace FluffyBarkFriendsWebApp.Models.Database;

public partial class ArchivedRecordVaccination
{
    public int ArchiveVaccinationId { get; set; }

    public int PetId { get; set; }

    public int AppointmentId { get; set; }

    public string VaccineName { get; set; } = null!;

    public DateOnly DateGiven { get; set; }

    public DateOnly? NextDueDate { get; set; }

    public string? Dose { get; set; }

    public string? Route { get; set; }

    public string? Remarks { get; set; }

    public int RecordedByUserId { get; set; }

    public DateTime OriginalCreatedAt { get; set; }

    public DateTime ArchivedAt { get; set; }

}
