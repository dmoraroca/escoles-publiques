using Application.Interfaces;
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
        var result = fees.Select(f =>
        {
            var first = f.Enrollment?.Student?.User?.FirstName ?? string.Empty;
            var last = f.Enrollment?.Student?.User?.LastName ?? string.Empty;
            var studentName = $"{first} {last}".Trim();
            if (string.IsNullOrWhiteSpace(studentName)) studentName = "Desconegut";

            return new AnnualFeeDtoOut(
                f.Id,
                studentName,
                f.Amount,
                f.Currency,
                f.DueDate,
                f.PaidAt.HasValue
            );
        });
        return Ok(result);
    }
}

public record AnnualFeeDtoOut(long Id, string StudentName, decimal Amount, string Currency, DateOnly DueDate, bool IsPaid);
