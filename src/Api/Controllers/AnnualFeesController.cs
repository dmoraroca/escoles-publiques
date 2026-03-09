using Application.Interfaces;
using Api.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Exposes HTTP endpoints to manage annual fees workflows.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnualFeesController : ControllerBase
{
    private readonly IAnnualFeeService _annualFeeService;
    /// <summary>
    /// Initializes a new instance of the AnnualFeesController class with its required dependencies.
    /// </summary>
    public AnnualFeesController(IAnnualFeeService annualFeeService)
    {
        _annualFeeService = annualFeeService;
    }
    /// <summary>
    /// Retrieves all and returns it to the caller.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var fees = await _annualFeeService.GetAllAnnualFeesAsync();
        return Ok(fees.Select(ToDto));
    }
    /// <summary>
    /// Retrieves the requested data and returns it to the caller.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var fee = await _annualFeeService.GetAnnualFeeByIdAsync(id);
        return Ok(ToDto(fee!));
    }
    /// <summary>
    /// Creates a new resource by applying the required business rules.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AnnualFeeDtoIn dto)
    {
        var fee = new AnnualFee
        {
            EnrollmentId = dto.EnrollmentId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            DueDate = dto.DueDate,
            PaidAt = dto.IsPaid ? DateTime.UtcNow : null,
            PaymentRef = dto.PaymentRef
        };
        var created = await _annualFeeService.CreateAnnualFeeAsync(fee);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, ToDto(created));
    }
    /// <summary>
    /// Updates the target resource with the data received in the request.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] AnnualFeeDtoIn dto)
    {
        var fee = await _annualFeeService.GetAnnualFeeByIdAsync(id);
        if (fee == null) return NotFound();

        fee.EnrollmentId = dto.EnrollmentId;
        fee.Amount = dto.Amount;
        fee.Currency = dto.Currency;
        fee.DueDate = dto.DueDate;
        fee.PaidAt = dto.IsPaid ? DateTime.UtcNow : null;
        fee.PaymentRef = dto.PaymentRef;

        await _annualFeeService.UpdateAnnualFeeAsync(fee);
        return NoContent();
    }
    /// <summary>
    /// Deletes the target resource from the system in a controlled manner.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _annualFeeService.DeleteAnnualFeeAsync(id);
        return NoContent();
    }
    /// <summary>
    /// Maps data for to dto between application layers.
    /// </summary>
    private static AnnualFeeDtoOut ToDto(AnnualFee fee)
    {
        var first = fee.Enrollment?.Student?.User?.FirstName ?? string.Empty;
        var last = fee.Enrollment?.Student?.User?.LastName ?? string.Empty;
        var studentName = $"{first} {last}".Trim();
        if (string.IsNullOrWhiteSpace(studentName)) studentName = "Alumne desconegut";
        var academicYear = fee.Enrollment?.AcademicYear ?? string.Empty;
        var courseName = fee.Enrollment?.CourseName;
        var enrollmentInfo = !string.IsNullOrWhiteSpace(academicYear)
            ? $"{studentName} - {academicYear}"
            : studentName;
        var schoolName = fee.Enrollment?.School?.Name ?? string.Empty;
        var schoolId = fee.Enrollment?.SchoolId;

        return new AnnualFeeDtoOut(
            fee.Id,
            fee.EnrollmentId,
            enrollmentInfo,
            studentName,
            academicYear,
            courseName,
            fee.Amount,
            fee.Currency,
            fee.DueDate,
            fee.PaidAt,
            fee.PaymentRef,
            schoolId,
            schoolName
        );
    }
}
