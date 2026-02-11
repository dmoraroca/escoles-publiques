
using Domain.DomainExceptions;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Microsoft.AspNetCore.Authorization;
using Web.Services.Api;
using Microsoft.Extensions.Localization;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador per gestionar les quotes anuals dels alumnes.
    /// </summary>
    [Authorize]
    public class AnnualFeesController : BaseController
    {
        private readonly IAnnualFeesApiClient _annualFeesApi;
        private readonly IEnrollmentsApiClient _enrollmentsApi;
        private readonly IStudentsApiClient _studentsApi;

        /// <summary>
        /// Constructor del controlador de quotes anuals.
        /// </summary>
        public AnnualFeesController(
            IAnnualFeesApiClient annualFeesApi,
            IEnrollmentsApiClient enrollmentsApi,
            IStudentsApiClient studentsApi,
            ILogger<AnnualFeesController> logger,
            IStringLocalizer<BaseController> localizer) : base(logger, localizer)
        {
            _annualFeesApi = annualFeesApi;
            _enrollmentsApi = enrollmentsApi;
            _studentsApi = studentsApi;
        }

        /// <summary>
        /// Mostra el formulari per crear una nova quota anual.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var enrollments = await _enrollmentsApi.GetAllAsync();
            var enrollmentVMs = enrollments
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .Select(e => new EnrollmentViewModel
                {
                    Id = (int)e.Id,
                    StudentId = (int)e.StudentId,
                    StudentName = e.StudentName,
                    AcademicYear = e.AcademicYear,
                    CourseName = e.CourseName,
                    EnrolledAt = e.EnrolledAt,
                    SchoolId = (int)e.SchoolId,
                    SchoolName = e.SchoolName ?? ""
                })
                .ToList();
            ViewBag.Enrollments = enrollmentVMs;

            // Afegir llista d'alumnes per al combo
            var students = await _studentsApi.GetAllAsync();
            ViewBag.Students = students.Select(s => new StudentViewModel
            {
                Id = (int)s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                SchoolId = s.SchoolId != 0 ? (int)s.SchoolId : 0,
                SchoolName = s.SchoolName ?? ""
            }).ToList();

            return View();
        }

        // ...existing code...

        /// <summary>
        /// Mostra el llistat de totes les quotes anuals.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var fees = await _annualFeesApi.GetAllAsync();
                var viewModels = fees.Select(f => new AnnualFeeViewModel
                {
                    Id = (int)f.Id,
                    EnrollmentId = (int)f.EnrollmentId,
                    EnrollmentInfo = f.EnrollmentInfo,
                    Amount = f.Amount,
                    Currency = f.Currency,
                    DueDate = f.DueDate,
                    PaidAt = f.PaidAt,
                    PaymentRef = f.PaymentRef,
                    AcademicYear = f.AcademicYear,
                    SchoolName = f.SchoolName ?? string.Empty,
                    SchoolId = (int)(f.SchoolId ?? 0)
                });

                var enrollments = await _enrollmentsApi.GetAllAsync();
                ViewBag.Enrollments = enrollments.Select(e => new EnrollmentViewModel
                {
                    Id = (int)e.Id,
                    StudentName = e.StudentName,
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

        /// <summary>
        /// Mostra els detalls d'una quota anual concreta.
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var fee = await _annualFeesApi.GetByIdAsync(id);
                if (fee == null)
                {
                    SetErrorMessage("No s'ha trobat la quota.");
                    return RedirectToAction("Index");
                }
                var viewModel = new AnnualFeeViewModel
                {
                    Id = (int)fee.Id,
                    EnrollmentId = (int)fee.EnrollmentId,
                    EnrollmentInfo = fee.EnrollmentInfo,
                    Amount = fee.Amount,
                    Currency = fee.Currency,
                    DueDate = fee.DueDate,
                    PaidAt = fee.PaidAt,
                    PaymentRef = fee.PaymentRef,
                    AcademicYear = fee.AcademicYear,
                    SchoolName = fee.SchoolName ?? string.Empty,
                    SchoolId = (int)(fee.SchoolId ?? 0)
                };
                var enrollments = await _enrollmentsApi.GetAllAsync();
                ViewBag.Enrollments = enrollments.Select(e => new EnrollmentViewModel
                {
                    Id = (int)e.Id,
                    StudentName = e.StudentName,
                    AcademicYear = e.AcademicYear
                }).ToList();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error obtenint detalls de la quota");
                SetErrorMessage("Error carregant la quota. Si us plau, intenta-ho de nou.");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        /// <summary>
        /// Processa la creació d'una nova quota anual.
        /// </summary>
        public async Task<IActionResult> Create(AnnualFeeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                    return RedirectToAction(nameof(Index));
                }

                var dto = new ApiAnnualFeeIn(
                    model.EnrollmentId,
                    model.Amount,
                    model.Currency ?? "EUR",
                    model.DueDate,
                    model.IsPaid,
                    model.PaymentRef
                );
                await _annualFeesApi.CreateAsync(dto);

                if (IsAjaxRequest())
                {
                    return Ok(new { message = "Quota creada correctament." });
                }
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
            catch (HttpRequestException ex) when (IsUnauthorized(ex))
            {
                Logger.LogWarning(ex, "Accés no autoritzat a l'API (crear quota)");
                if (IsAjaxRequest())
                {
                    return Unauthorized(new { error = "Accés no autoritzat. Torna a iniciar sessió." });
                }
                SetErrorMessage("Accés no autoritzat. Torna a iniciar sessió.");
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creant quota");
                SetErrorMessage("Error creant la quota. Si us plau, intenta-ho de nou.");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Mostra el formulari per editar una quota anual existent.
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var annualFee = await _annualFeesApi.GetByIdAsync(id);
                if (annualFee == null)
                {
                    SetErrorMessage($"Quota amb ID {id} no trobada.");
                    return RedirectToAction(nameof(Index));
                }
                var viewModel = new AnnualFeeViewModel
                {
                    Id = (int)annualFee.Id,
                    EnrollmentId = (int)annualFee.EnrollmentId,
                    EnrollmentInfo = annualFee.EnrollmentInfo,
                    Amount = annualFee.Amount,
                    Currency = annualFee.Currency,
                    DueDate = annualFee.DueDate,
                    IsPaid = annualFee.PaidAt.HasValue,
                    PaidAt = annualFee.PaidAt,
                    PaymentRef = annualFee.PaymentRef,
                    AcademicYear = annualFee.AcademicYear ?? string.Empty,
                    SchoolName = annualFee.SchoolName ?? string.Empty,
                    SchoolId = (int)(annualFee.SchoolId ?? 0)
                };
                // Carregar inscripcions per al dropdown
                var enrollments = await _enrollmentsApi.GetAllAsync();
                ViewBag.Enrollments = enrollments.Select(e => new EnrollmentViewModel
                {
                    Id = (int)e.Id,
                    StudentName = e.StudentName,
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
        /// <summary>
        /// Processa l'edició d'una quota anual existent.
        /// </summary>
        public async Task<IActionResult> Edit(AnnualFeeViewModel model)
        {
            try
            {
                Logger.LogInformation("=== EDIT POST ===");
                Logger.LogInformation("Amount rebut del formulari: {Amount}", model.Amount);
                Logger.LogInformation("Model.Amount tipus: {Type}", model.Amount.GetType());

                if (!ModelState.IsValid)
                {
                    var enrollments = await _enrollmentsApi.GetAllAsync();
                    ViewBag.Enrollments = enrollments.Select(e => new EnrollmentViewModel
                    {
                        Id = (int)e.Id,
                        StudentName = e.StudentName,
                        AcademicYear = e.AcademicYear,
                        CourseName = e.CourseName,
                        EnrolledAt = e.EnrolledAt
                    }).ToList();
                    SetErrorMessage("Si us plau, omple tots els camps obligatoris correctament.");
                    return View(model);
                }

                var dto = new ApiAnnualFeeIn(
                    model.EnrollmentId,
                    model.Amount,
                    model.Currency ?? "EUR",
                    model.DueDate,
                    model.IsPaid,
                    model.PaymentRef
                );
                await _annualFeesApi.UpdateAsync(model.Id, dto);

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
        /// <summary>
        /// Elimina una quota anual.
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _annualFeesApi.DeleteAsync(id);

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

        /// <summary>
        /// Mètode temporal per corregir imports erronis de quotes anuals.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FixAmounts()
        {
            try
            {
                var allFees = await _annualFeesApi.GetAllAsync();
                int count = 0;

                foreach (var fee in allFees)
                {
                    // Si l'import és massa gran (probablement multiplicat per 100)
                    if (fee.Amount > 10000)
                    {
                        var dto = new ApiAnnualFeeIn(
                            fee.EnrollmentId,
                            fee.Amount / 100,
                            fee.Currency,
                            fee.DueDate,
                            fee.PaidAt.HasValue,
                            fee.PaymentRef
                        );
                        await _annualFeesApi.UpdateAsync(fee.Id, dto);
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
}
