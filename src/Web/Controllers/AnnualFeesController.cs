using Application.Interfaces;
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

[Authorize]
public class AnnualFeesController : BaseController
{
    private readonly IAnnualFeeService _annualFeeService;
    private readonly IEnrollmentService _enrollmentService;

    public AnnualFeesController(
        IAnnualFeeService annualFeeService,
        IEnrollmentService enrollmentService,
        ILogger<AnnualFeesController> logger) : base(logger)
    {
        _annualFeeService = annualFeeService;
        _enrollmentService = enrollmentService;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            var fees = await _annualFeeService.GetAllAnnualFeesAsync();
            var viewModels = fees.Select(f => new AnnualFeeViewModel
            {
                Id = (int)f.Id,
                EnrollmentInfo = f.Enrollment?.Student?.User != null 
                    ? $"{f.Enrollment.Student.User.FirstName} {f.Enrollment.Student.User.LastName} - {f.Enrollment.AcademicYear}"
                    : "Inscripció desconeguda",
                Amount = f.Amount,
                Currency = f.Currency,
                DueDate = f.DueDate.ToDateTime(TimeOnly.MinValue),
                PaidAt = f.PaidAt,
                PaymentRef = f.PaymentRef
            });
            
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            ViewBag.Enrollments = enrollments.Select(e => new EnrollmentViewModel
            {
                Id = (int)e.Id,
                StudentName = e.Student?.User != null ? $"{e.Student.User.FirstName} {e.Student.User.LastName}" : "Alumne desconegut",
                AcademicYear = e.AcademicYear
            }).ToList();
            
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
                EnrollmentId = (int)fee.EnrollmentId,
                EnrollmentInfo = fee.Enrollment?.Student?.User != null 
                    ? $"{fee.Enrollment.Student.User.FirstName} {fee.Enrollment.Student.User.LastName} - {fee.Enrollment.AcademicYear}"
                    : "Inscripció desconeguda",
                Amount = fee.Amount,
                Currency = fee.Currency,
                DueDate = fee.DueDate.ToDateTime(TimeOnly.MinValue),
                PaidAt = fee.PaidAt,
                PaymentRef = fee.PaymentRef
            };
            
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            ViewBag.Enrollments = enrollments.Select(e => new EnrollmentViewModel
            {
                Id = (int)e.Id,
                StudentName = e.Student?.User != null ? $"{e.Student.User.FirstName} {e.Student.User.LastName}" : "Alumne desconegut",
                AcademicYear = e.AcademicYear
            }).ToList();
            
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
    
    public async Task<IActionResult> Create()
    {
        var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
        ViewBag.Enrollments = enrollments.Select(e => new EnrollmentViewModel
        {
            Id = (int)e.Id,
            StudentName = e.Student?.User != null ? $"{e.Student.User.FirstName} {e.Student.User.LastName}" : "Alumne desconegut",
            AcademicYear = e.AcademicYear,
            CourseName = e.CourseName,
            EnrolledAt = e.EnrolledAt
        }).ToList();
        
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AnnualFeeViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                return RedirectToAction(nameof(Index));
            }

            var annualFee = new Domain.Entities.AnnualFee
            {
                EnrollmentId = model.EnrollmentId,
                Amount = model.Amount,
                Currency = model.Currency ?? "EUR",
                DueDate = DateOnly.FromDateTime(model.DueDate),
                PaidAt = model.IsPaid ? DateTime.Now : null,
                PaymentRef = model.PaymentRef
            };
            
            await _annualFeeService.CreateAnnualFeeAsync(annualFee);
            
            SetSuccessMessage("Quota creada correctament.");
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Inscripció no trobada al crear quota");
            SetErrorMessage("La inscripció seleccionada no existeix.");
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al crear quota");
            SetErrorMessage($"Error de validació: {string.Join(", ", ex.Errors.SelectMany(e => e.Value))}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creant quota");
            SetErrorMessage("Error creant la quota. Si us plau, intenta-ho de nou.");
            return RedirectToAction(nameof(Index));
        }
    }
    
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var annualFee = await _annualFeeService.GetAnnualFeeByIdAsync(id);
            
            var viewModel = new AnnualFeeViewModel
            {
                Id = (int)annualFee.Id,
                EnrollmentId = (int)annualFee.EnrollmentId,
                EnrollmentInfo = annualFee.Enrollment?.Student?.User != null 
                    ? $"{annualFee.Enrollment.Student.User.FirstName} {annualFee.Enrollment.Student.User.LastName} - {annualFee.Enrollment.AcademicYear}"
                    : "Inscripció desconeguda",
                Amount = annualFee.Amount,
                Currency = annualFee.Currency,
                DueDate = annualFee.DueDate.ToDateTime(TimeOnly.MinValue),
                IsPaid = annualFee.PaidAt.HasValue,
                PaidAt = annualFee.PaidAt,
                PaymentRef = annualFee.PaymentRef
            };
            
            // Carregar inscripcions per al dropdown
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            ViewBag.Enrollments = enrollments.Select(e => new EnrollmentViewModel
            {
                Id = (int)e.Id,
                StudentName = e.Student?.User != null ? $"{e.Student.User.FirstName} {e.Student.User.LastName}" : "Alumne desconegut",
                AcademicYear = e.AcademicYear,
                CourseName = e.CourseName,
                EnrolledAt = e.EnrolledAt
            }).ToList();
            
            return View(viewModel);
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Quota amb Id {Id} no trobada per editar", id);
            SetErrorMessage($"Quota amb ID {id} no trobada.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error carregant quota per editar {Id}", id);
            SetErrorMessage("Error carregant la quota.");
            return RedirectToAction(nameof(Index));
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AnnualFeeViewModel model)
    {
        try
        {
            Logger.LogInformation("=== EDIT POST ===");
            Logger.LogInformation("Amount rebut del formulari: {Amount}", model.Amount);
            Logger.LogInformation("Model.Amount tipus: {Type}", model.Amount.GetType());
            
            if (!ModelState.IsValid)
            {
                // Recarregar inscripcions si hi ha errors
                var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
                ViewBag.Enrollments = enrollments.Select(e => new EnrollmentViewModel
                {
                    Id = (int)e.Id,
                    StudentName = e.Student?.User != null ? $"{e.Student.User.FirstName} {e.Student.User.LastName}" : "Alumne desconegut",
                    AcademicYear = e.AcademicYear,
                    CourseName = e.CourseName,
                    EnrolledAt = e.EnrolledAt
                }).ToList();
                
                SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                return View(model);
            }

            var annualFee = await _annualFeeService.GetAnnualFeeByIdAsync(model.Id);
            
            Logger.LogInformation("Amount ABANS d'actualitzar: {OldAmount}", annualFee.Amount);
            
            // Actualitzar tots els camps exactament com en Create
            annualFee.EnrollmentId = model.EnrollmentId;
            annualFee.Amount = model.Amount;
            annualFee.Currency = model.Currency ?? "EUR";
            annualFee.DueDate = DateOnly.FromDateTime(model.DueDate);
            annualFee.PaidAt = model.IsPaid ? DateTime.Now : null;
            annualFee.PaymentRef = model.PaymentRef;

            Logger.LogInformation("Amount DESPRÉS d'assignar: {NewAmount}", annualFee.Amount);

            await _annualFeeService.UpdateAnnualFeeAsync(annualFee);
            
            Logger.LogInformation("Quota actualitzada correctament");
            
            SetSuccessMessage("Quota actualitzada correctament.");
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Quota o inscripció no trobades al actualitzar");
            SetErrorMessage("La quota o la inscripció no existeixen.");
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(ex, "Error de validació al actualitzar quota");
            SetErrorMessage($"Error de validació: {string.Join(", ", ex.Errors.SelectMany(e => e.Value))}");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error actualitzant quota {Id}", model.Id);
            SetErrorMessage("Error al actualitzar la quota. Si us plau, intenta-ho de nou.");
            return RedirectToAction(nameof(Index));
        }
    }
    
    [HttpPost]
[HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _annualFeeService.DeleteAnnualFeeAsync(id);
            
            SetSuccessMessage("Quota esborrada correctament.");
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException ex)
        {
            Logger.LogWarning(ex, "Intent d'esborrar quota inexistent: {Id}", id);
            SetErrorMessage("La quota no existeix.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error esborrant quota {Id}", id);
            SetErrorMessage("Error al esborrar la quota.");
            return RedirectToAction(nameof(Index));
        }
    }
    
    // MÈTODE TEMPORAL PER CORREGIR IMPORTS
    [HttpGet]
    public async Task<IActionResult> FixAmounts()
    {
        try
        {
            var allFees = await _annualFeeService.GetAllAnnualFeesAsync();
            int count = 0;
            
            foreach (var fee in allFees)
            {
                // Si l'import és massa gran (probablement multiplicat per 100)
                if (fee.Amount > 10000)
                {
                    fee.Amount = fee.Amount / 100;
                    await _annualFeeService.UpdateAnnualFeeAsync(fee);
                    count++;
                }
            }
            
            SetSuccessMessage($"{count} quotes corregides correctament.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error corregint imports de quotes");
            SetErrorMessage("Error al corregir les quotes.");
            return RedirectToAction(nameof(Index));
        }
    }
}
