using Microsoft.AspNetCore.Mvc;
using Tutorial5.DTOs;
using Tutorial5.Services;

namespace Tutorial5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrescriptionsController : ControllerBase
{
    private readonly IDbService _dbService;

    public PrescriptionsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionRequestDto dto)
    {
        var result = await _dbService.AddPrescription(dto);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = result.Message });
    }
    
    [HttpGet("/api/patients/{id}")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var result = await _dbService.GetPatientWithPrescriptionsAsync(id);
        if (result is null)
            return NotFound(new { message = $"pacjent o id {id} nie istnieje" });

        return Ok(result);
    }

}