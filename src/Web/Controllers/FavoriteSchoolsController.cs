using Microsoft.AspNetCore.Mvc;
using Web.Models;

using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers;

/// <summary>
/// Controlador per mostrar les escoles preferides de l'usuari.
/// </summary>
public class FavoriteSchoolsController : Controller
{
    /// <summary>
    /// Mostra la llista d'escoles preferides com a partial view.
    /// </summary>
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
