using Microsoft.EntityFrameworkCore;
using Tutorial5.Data;
using Tutorial5.DTOs;
using Tutorial5.Models;

namespace Tutorial5.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;

    public DbService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message)> AddPrescription(PrescriptionRequestDto dto)
    {
        if (dto.Medicaments.Count > 10)
            return (false, "recepta moze zawierac maksymalnie 10 lekow");

        if (dto.DueDate < dto.Date)
            return (false, "data realizacji (DueDate) nie moze byc wczesniejsza niz data wystawienia");

        var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
        if (doctor == null)
            return (false, $"lekarz o id {dto.DoctorId} nie  istnieje");

        var medicamentIds = dto.Medicaments.Select(m => m.IdMedicament).ToList();
        var existingMedicaments = await _context.Medicaments
            .Where(m => medicamentIds.Contains(m.IdMedicament))
            .Select(m => m.IdMedicament)
            .ToListAsync();

        if (existingMedicaments.Count != medicamentIds.Count)
            return (false, "co najmniej jeden z podanych lekow nie istnieje");

        var patient = await _context.Patients.FirstOrDefaultAsync(p =>
            p.FirstName == dto.PatientFirstName &&
            p.LastName == dto.PatientLastName &&
            p.Birthdate == dto.PatientBirthdate);

        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = dto.PatientFirstName,
                LastName = dto.PatientLastName,
                Birthdate = dto.PatientBirthdate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        var prescription = new Prescription
        {
            Date = dto.Date,
            DueDate = dto.DueDate,
            IdDoctor = dto.DoctorId,
            IdPatient = patient.IdPatient,
            PrescriptionMedicaments = dto.Medicaments.Select(m => new PrescriptionMedicament
            {
                IdMedicament = m.IdMedicament,
                Dose = m.Dose,
                Description = m.Description
            }).ToList()
        };

        _context.Prescriptions.Add(prescription);
        await _context.SaveChangesAsync();

        return (true, "recepta zostala poprawnie dodana");
    }
    
    public async Task<PatientDto?> GetPatientWithPrescriptionsAsync(int id)
    {
        var patient = await _context.Patients
            .Where(p => p.IdPatient == id)
            .Select(p => new PatientDto
            {
                IdPatient = p.IdPatient,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Birthdate = p.Birthdate,
                Prescriptions = p.Prescriptions
                    .OrderBy(p => p.DueDate)
                    .Select(r => new PrescriptionDto
                    {
                        IdPrescription = r.IdPrescription,
                        Date = r.Date,
                        DueDate = r.DueDate,
                        Doctor = new DoctorDto
                        {
                            IdDoctor = r.Doctor.IdDoctor,
                            FirstName = r.Doctor.FirstName,
                            LastName = r.Doctor.LastName,
                            Email = r.Doctor.Email
                        },
                        Medicaments = r.PrescriptionMedicaments.Select(m => new PrescriptionMedicamentDto
                        {
                            IdMedicament = m.IdMedicament,
                            Dose = m.Dose,
                            Description = m.Description
                        }).ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        return patient;
    }

    
}
