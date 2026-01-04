using Application.Interfaces;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class AnnualFeesController : BaseController
{
    private readonly IAnnualFeeService _annualFeeService;

    public AnnualFeesController(IAnnualFeeService annualFeeService, ILogger<AnnualFeesController> logger) : base(logger)
    {
        _annualFeeService = annualFeeService;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            var fees = await _annualFeeService.GetAllAnnualFeesAsync();
            var viewModels = fees.Select(f => new AnnualFeeViewModel
            {
                Id = (int)f.Id,
                EnrollmentInfo = f.Enrollment?.Student != null 
                    ? $"{f.Enrollment.Student.FirstName} {f.Enrollment.Student.LastName} - {f.Enrollment.AcademicYear}"
                    : "Inscripció desconeguda",
                Amount = f.Amount,
                Currency = f.Currency,
                DueDate = f.DueDate.ToDateTime(TimeOnly.MinValue),
                PaidAt = f.PaidAt,
                PaymentRef = f.PaymentRef
            });
            
            return View(viewModels);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint llista de quotes");
            SetErrorMessage("Error carregant les quotes. Si us plau, intenta-ho de nou.");
            return View(new List<AnnualFeeViewModel>());
        }
    }
    
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var fee = await _annualFeeService.GetAnnualFeeByIdAsync(id);
            
            var viewModel = new AnnualFeeViewModel
            {
                Id = (int)fee.Id,
                EnrollmentInfo = fee.Enrollment?.Student != null 
                    ? $"{fee.Enrollment.Student.FirstName} {fee.Enrollment.Student.LastName} - {fee.Enrollment.AcademicYear}"
                    : "Inscripció desconeguda",
                Amount = fee.Amount,
                Currency = fee.Currency,
                DueDate = fee.DueDate.ToDateTime(TimeOnly.MinValue),
                PaidAt = fee.PaidAt,
                PaymentRef = fee.PaymentRef
            };
            
            return View(viewModel);
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Quota amb Id {Id} no trobada", id);
            SetErrorMessage($"Quota amb ID {id} no trobada.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error obtenint detalls de la quota {Id}", id);
            SetErrorMessage("Error carregant els detalls de la quota.");
            return RedirectToAction(nameof(Index));
        }
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(int enrollmentId, decimal amount, DateTime dueDate)
    {
        // TODO: Crear nova quota
        return RedirectToAction(nameof(Index));
    }
}
