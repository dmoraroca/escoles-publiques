using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class AnnualFeesController : BaseController
{
    public AnnualFeesController(ILogger<AnnualFeesController> logger) : base(logger)
    {
    }
    
    public IActionResult Index()
    {
        var fees = new List<AnnualFeeViewModel>
        {
            new AnnualFeeViewModel { Id = 1, EnrollmentInfo = "Marc García - 2025-2026", Amount = 1500.00m, Currency = "EUR", DueDate = new DateTime(2025, 10, 31), PaidAt = new DateTime(2025, 10, 15), PaymentRef = "PAY-001-2025" },
            new AnnualFeeViewModel { Id = 2, EnrollmentInfo = "Laura Martínez - 2025-2026", Amount = 1500.00m, Currency = "EUR", DueDate = new DateTime(2025, 10, 31), PaidAt = null, PaymentRef = null },
            new AnnualFeeViewModel { Id = 3, EnrollmentInfo = "Pau Sánchez - 2024-2025", Amount = 1450.00m, Currency = "EUR", DueDate = new DateTime(2024, 10, 31), PaidAt = new DateTime(2024, 10, 20), PaymentRef = "PAY-045-2024" },
            new AnnualFeeViewModel { Id = 4, EnrollmentInfo = "Anna Ferrer - 2025-2026", Amount = 1500.00m, Currency = "EUR", DueDate = new DateTime(2025, 11, 15), PaidAt = null, PaymentRef = null },
            new AnnualFeeViewModel { Id = 5, EnrollmentInfo = "Joan Roca - 2025-2026", Amount = 1500.00m, Currency = "EUR", DueDate = new DateTime(2025, 10, 31), PaidAt = new DateTime(2025, 11, 5), PaymentRef = "PAY-012-2025" }
        };
        
        return View(fees);
    }
    
    public IActionResult Details(int id)
    {
        // TODO: Carregar detalls d'una quota
        return View();
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
