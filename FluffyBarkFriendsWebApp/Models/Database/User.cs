using System;
using System.Collections.Generic;

namespace FluffyBarkFriendsWebApp.Models.Database;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;
    public string Username { get; internal set; }
    public string FullName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Appointment> Appointment { get; set; } = new List<Appointment>();

    public virtual ICollection<Vaccination> VaccinationRecord { get; set; } = new List<Vaccination>();
}
