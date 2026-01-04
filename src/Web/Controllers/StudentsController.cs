using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class StudentsController : BaseController
{
    public StudentsController(ILogger<StudentsController> logger) : base(logger)
    {
    }
    
    public IActionResult Index()
    {
        var students = new List<StudentViewModel>
        {
            new StudentViewModel { Id = 1, FirstName = "Marc", LastName = "García López", Email = "marc.garcia@email.cat", BirthDate = new DateTime(2010, 5, 15), SchoolName = "Escola Pia de Sarrià" },
            new StudentViewModel { Id = 2, FirstName = "Laura", LastName = "Martínez Rodríguez", Email = "laura.martinez@email.cat", BirthDate = new DateTime(2011, 8, 22), SchoolName = "Institut Montserrat" },
            new StudentViewModel { Id = 3, FirstName = "Pau", LastName = "Sánchez Pujol", Email = "pau.sanchez@email.cat", BirthDate = new DateTime(2009, 3, 10), SchoolName = "Col·legi Sagrada Família" },
            new StudentViewModel { Id = 4, FirstName = "Anna", LastName = "Ferrer Vidal", Email = "anna.ferrer@email.cat", BirthDate = new DateTime(2012, 11, 5), SchoolName = "Escola Joan Maragall" },
            new StudentViewModel { Id = 5, FirstName = "Joan", LastName = "Roca Casals", Email = "joan.roca@email.cat", BirthDate = new DateTime(2010, 7, 18), SchoolName = "Institut Comte de Rius" }
        };
        
        return View(students);
    }
    
    public IActionResult Details(int id)
    {
        // TODO: Carregar detalls d'un alumne
        return View();
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(string firstName, string lastName, string email)
    {
        // TODO: Crear nou alumne
        return RedirectToAction(nameof(Index));
    }
}
