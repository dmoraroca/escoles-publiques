using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class FavoriteSchoolsController : Controller
{
    public IActionResult Index()
    {
        var favoriteSchools = new List<FavoriteSchoolViewModel>
        {
            new FavoriteSchoolViewModel { Id = 1, Name = "Escola Pia de Sarrià", Municipality = "Barcelona", Url = "#" },
            new FavoriteSchoolViewModel { Id = 2, Name = "Institut Montserrat", Municipality = "Barcelona", Url = "#" },
            new FavoriteSchoolViewModel { Id = 3, Name = "Col·legi Sagrada Família", Municipality = "Sabadell", Url = "#" },
            new FavoriteSchoolViewModel { Id = 4, Name = "Escola Joan Maragall", Municipality = "Girona", Url = "#" }
        };
        
        return PartialView("_FavoriteSchools", favoriteSchools);
    }
}
