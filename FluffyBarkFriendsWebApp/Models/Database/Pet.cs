using System;
using System.Collections.Generic;

namespace FluffyBarkFriendsWebApp.Models.Database;

public partial class Pet
{
    public int PetId { get; set; }

    public string PetName { get; set; } = null!;

    public string Species { get; set; } = null!;

    public string? Breed { get; set; }

    public string Sex { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public int? AgeYears { get; set; }

    public string? Color { get; set; }

    public decimal? Weight { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? OwnerName { get; set; }

    public int? OwnerUserId { get; set; }

    public string? ContactNumber { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    public virtual User? OwnerUser { get; set; }

    public virtual ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
}
