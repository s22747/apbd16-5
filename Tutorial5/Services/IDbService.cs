using Tutorial5.DTOs;

namespace Tutorial5.Services;

public interface IDbService
{
    Task<(bool Success, string Message)> AddPrescription(PrescriptionRequestDto dto);
    
    Task<PatientDto?> GetPatientWithPrescriptionsAsync(int id);
    
}