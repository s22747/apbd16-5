using Microsoft.EntityFrameworkCore;
using Tutorial5.Models;

namespace Tutorial5.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

    protected DatabaseContext() { }

    public DatabaseContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PATIENT
        modelBuilder.Entity<Patient>(p =>
        {
            p.ToTable("Patient");
            p.HasKey(e => e.IdPatient);
            p.Property(e => e.FirstName).HasMaxLength(100);
            p.Property(e => e.LastName).HasMaxLength(100);
        });

        // DOCTOR
        modelBuilder.Entity<Doctor>(d =>
        {
            d.ToTable("Doctor");
            d.HasKey(e => e.IdDoctor);
            d.Property(e => e.FirstName).HasMaxLength(100);
            d.Property(e => e.LastName).HasMaxLength(100);
            d.Property(e => e.Email).HasMaxLength(100);
        });

        // MEDICAMENT
        modelBuilder.Entity<Medicament>(m =>
        {
            m.ToTable("Medicament");
            m.HasKey(e => e.IdMedicament);
            m.Property(e => e.Name).HasMaxLength(100);
            m.Property(e => e.Description).HasMaxLength(100);
            m.Property(e => e.Type).HasMaxLength(100);
        });

        // PRESCRIPTION
        modelBuilder.Entity<Prescription>(p =>
        {
            p.ToTable("Prescription");
            p.HasKey(e => e.IdPrescription);
        });

        // PRESCRIPTION_MEDICAMENT
        modelBuilder.Entity<PrescriptionMedicament>(pm =>
        {
            pm.ToTable("Prescription_Medicament");
            pm.HasKey(e => new { e.IdPrescription, e.IdMedicament });
            pm.Property(e => e.Description).HasMaxLength(100);
        });

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Medicament)
            .WithMany(m => m.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdMedicament);

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdPrescription);

        // SEED DATA
        modelBuilder.Entity<Doctor>().HasData(new List<Doctor>
        {
            new Doctor { IdDoctor = 1, FirstName = "grzegorz", LastName = "braun", Email = "braun@gmail.com" },
            new Doctor { IdDoctor = 2, FirstName = "adrian", LastName = "zandberg", Email = "adekz@wp.pl" }
        });

        modelBuilder.Entity<Patient>().HasData(new List<Patient>
        {
            new Patient { IdPatient = 1, FirstName = "ania", LastName = "kowal", Birthdate = new DateTime(1990, 1, 1) },
            new Patient { IdPatient = 2, FirstName = "piotrek", LastName = "nowak", Birthdate = new DateTime(1985, 5, 12) }
        });

        modelBuilder.Entity<Medicament>().HasData(new List<Medicament>
        {
            new Medicament { IdMedicament = 1, Name = "paracetamol", Description = "przeciwbolowy", Type = "tabletka" },
            new Medicament { IdMedicament = 2, Name = "ibuprofen", Description = "przeciwzapalny", Type = "kapsulka" }
        });

        modelBuilder.Entity<Prescription>().HasData(new List<Prescription>
        {
            new Prescription { IdPrescription = 1, Date = new DateTime(2024, 5, 1), DueDate = new DateTime(2024, 5, 15), IdDoctor = 1, IdPatient = 1 },
            new Prescription { IdPrescription = 2, Date = new DateTime(2024, 5, 2), DueDate = new DateTime(2024, 5, 20), IdDoctor = 2, IdPatient = 2 }
        });

        modelBuilder.Entity<PrescriptionMedicament>().HasData(new List<PrescriptionMedicament>
        {
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 1, Dose = 2, Description = "prosze brac dwie dziennie" },
            new PrescriptionMedicament { IdPrescription = 1, IdMedicament = 2, Dose = 1, Description = "przed snem" },
            new PrescriptionMedicament { IdPrescription = 2, IdMedicament = 1, Dose = 1, Description = "jenda dizennie" }
        });
    }
}
