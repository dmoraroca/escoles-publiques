using Application.Interfaces;
using Api.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnnualFeesController : ControllerBase
{
    private readonly IAnnualFeeService _annualFeeService;

    public AnnualFeesController(IAnnualFeeService annualFeeService)
    {
        _annualFeeService = annualFeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var fees = await _annualFeeService.GetAllAnnualFeesAsync();
        return Ok(fees.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var fee = await _annualFeeService.GetAnnualFeeByIdAsync(id);
        return Ok(ToDto(fee!));
    }

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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _annualFeeService.DeleteAnnualFeeAsync(id);
        return NoContent();
    }

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
