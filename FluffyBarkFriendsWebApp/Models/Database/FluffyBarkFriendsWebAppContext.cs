using System;
using System.Collections.Generic;
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
    {
        optionsBuilder.UseSqlServer("Server=LAPTOP-7M7ONF2K;Database=FluffyBarkFriendsWebApp;Trusted_Connection=True;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentTime).HasPrecision(0);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReasonVisit)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Remarks)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ArchivedRecordVaccination>(entity =>
        {
            entity.HasKey(e => e.ArchiveVaccinationId);

            entity.ToTable("ArchivedRecordVaccination");

            entity.Property(e => e.ArchivedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dose)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.OriginalCreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Remarks)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Route)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VaccineName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.HasKey(e => e.MedicalHistoryId);

            entity.ToTable("MedicalHistory");

            entity.Property(e => e.MedicalHistoryId)
                .HasColumnName("MedicalHistory");

            entity.Property(e => e.Condition)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Diagnosis)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Dosage)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Medication)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Notes)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Treatment)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(e => e.Pet)
                .WithMany()
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(e => e.RecordedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("Pet");

            entity.Property(e => e.Breed)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Color)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PetName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Sex)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Species)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FullName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Vaccination>(entity =>
        {
            entity.ToTable("Vaccination");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dose)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Remarks)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.VaccineName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
