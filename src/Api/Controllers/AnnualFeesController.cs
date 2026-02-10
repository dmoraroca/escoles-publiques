using Application.Interfaces;
using Domain.DomainExceptions;
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
    private readonly ILogger<AnnualFeesController> _logger;

    public AnnualFeesController(IAnnualFeeService annualFeeService, ILogger<AnnualFeesController> logger)
    {
        _annualFeeService = annualFeeService;
        _logger = logger;
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
        try
        {
            var fee = await _annualFeeService.GetAnnualFeeByIdAsync(id);
            return Ok(ToDto(fee!));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AnnualFeeDtoIn dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
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
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Count > 0
                ? ex.Errors
                : new Dictionary<string, string[]> { { "Validation", new[] { ex.Message } } };
            return ValidationProblem(new ValidationProblemDetails(errors));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating annual fee");
            return Problem(detail: ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] AnnualFeeDtoIn dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
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
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Count > 0
                ? ex.Errors
                : new Dictionary<string, string[]> { { "Validation", new[] { ex.Message } } };
            return ValidationProblem(new ValidationProblemDetails(errors));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating annual fee {Id}", id);
            return Problem(detail: ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _annualFeeService.DeleteAnnualFeeAsync(id);
            return NoContent();
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting annual fee {Id}", id);
            return Problem(detail: ex.Message);
        }
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

public record AnnualFeeDtoIn(
    long EnrollmentId,
    decimal Amount,
    string Currency,
    DateOnly DueDate,
    bool IsPaid,
    string? PaymentRef);

public record AnnualFeeDtoOut(
    long Id,
    long EnrollmentId,
    string EnrollmentInfo,
    string StudentName,
    string AcademicYear,
    string? CourseName,
    decimal Amount,
    string Currency,
    DateOnly DueDate,
    DateTime? PaidAt,
    string? PaymentRef,
    long? SchoolId,
    string? SchoolName);
