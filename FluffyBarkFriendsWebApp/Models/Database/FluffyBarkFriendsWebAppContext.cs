using Microsoft.EntityFrameworkCore;

namespace FluffyBarkFriendsWebApp.Models.Database;

public partial class FluffyBarkFriendsWebAppContext : DbContext
{
    public FluffyBarkFriendsWebAppContext()
    {
    }

    public FluffyBarkFriendsWebAppContext(DbContextOptions<FluffyBarkFriendsWebAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }
    public virtual DbSet<ArchivedRecordVaccination> ArchivedRecordVaccinations { get; set; }
    public virtual DbSet<MedicalHistory> MedicalHistories { get; set; }
    public virtual DbSet<Pet> Pets { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Vaccination> Vaccinations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=EA512-07;Database=FluffyBarkFriendsWebApp;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.HasKey(e => e.MedicalHistoryId);

            entity.ToTable("MedicalHistory");

            entity.Property(e => e.Condition).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.Diagnosis).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Dosage).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Medication).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Treatment).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.VisitTime).HasPrecision(0);

            entity.HasOne(d => d.CreatedByUser)
                .WithMany(p => p.MedicalHistories)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedicalHistory_User");

            entity.HasOne(d => d.Pet)
                .WithMany(p => p.MedicalHistories)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedicalHistory_Pet");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentTime).HasPrecision(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.ReasonVisit).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Remarks).HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.Status).HasMaxLength(100).IsUnicode(false);

            entity.HasOne(d => d.CreatedByUser)
                .WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_User");

            entity.HasOne(d => d.Pet)
                .WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Pet");
        });

        modelBuilder.Entity<Vaccination>(entity =>
        {
            entity.ToTable("Vaccination");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.Dose).HasMaxLength(30).IsUnicode(false);
            entity.Property(e => e.Remarks).HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.VaccineName).HasMaxLength(100).IsUnicode(false);

            entity.HasOne(d => d.Appointment)
                .WithMany(p => p.Vaccinations)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vaccination_Appointment");

            entity.HasOne(d => d.Pet)
                .WithMany(p => p.Vaccinations)
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vaccination_Pet");

            entity.HasOne(d => d.RecordedByUser)
                .WithMany(p => p.Vaccinations)
                .HasForeignKey(d => d.RecordedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vaccination_User");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("Pet");

            entity.Property(e => e.Breed).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Color).HasMaxLength(30).IsUnicode(false);
            entity.Property(e => e.ContactNumber).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Notes).HasMaxLength(255).IsUnicode(false);
            entity.Property(e => e.OwnerName).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.PetName).HasMaxLength(30).IsUnicode(false);
            entity.Property(e => e.Sex).HasMaxLength(30).IsUnicode(false);
            entity.Property(e => e.Species).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.OwnerUser)
                .WithMany(p => p.Pets)
                .HasForeignKey(d => d.OwnerUserId)
                .HasConstraintName("FK_Pet_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.UserName).IsUnique();

            entity.Property(e => e.Contact).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(30).IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Role).HasMaxLength(30).IsUnicode(false);
            entity.Property(e => e.UserName).HasMaxLength(30).IsUnicode(false);
        });

        modelBuilder.Entity<ArchivedRecordVaccination>(entity =>
        {
            entity.HasKey(e => e.ArchiveVaccinationId);

            entity.ToTable("ArchivedRecordVaccination");

            entity.Property(e => e.ArchivedAt).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.Dose).HasMaxLength(30).IsUnicode(false);
            entity.Property(e => e.OriginalCreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(150).IsUnicode(false);
            entity.Property(e => e.Route).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.VaccineName).HasMaxLength(100).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}