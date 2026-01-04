using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class SchoolsController : BaseController
{
    private readonly ISchoolService _schoolService;

    public SchoolsController(ISchoolService schoolService, ILogger<SchoolsController> logger) : base(logger)
    {
        _schoolService = schoolService;
    }
    
    public async Task<IActionResult> Index()
    {
        try
        {
            var schools = await _schoolService.GetAllSchoolsAsync();
            var viewModels = schools.Select(s => new SchoolViewModel
            {
                Id = (int)s.Id,
                Code = s.Code,
                Name = s.Name,
                City = s.City ?? "",
                CreatedAt = s.CreatedAt
            });
            
            return View(viewModels);
        }
        catch (Exception ex)
        {
            return HandleError(ex, nameof(Index));
        }
    }
    
    public IActionResult Details(int id)
    {
        // TODO: Carregar detalls d'una escola
        return View();
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(string name, string code, string city)
    {
        // TODO: Crear nova escola
        return RedirectToAction(nameof(Index));
    }
}
