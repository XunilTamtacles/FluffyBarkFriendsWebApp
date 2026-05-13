using System;
using System.Collections.Generic;

namespace FluffyBarkFriendsWebApp.Models.Database;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Contact { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public virtual ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
}
