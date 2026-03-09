using Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Web.Controllers
{
    /// <summary>
    /// Exposes HTTP endpoints to manage home workflows.
    /// </summary>
    public partial class HomeController : BaseController
    {
        /// <summary>
        /// Retrieves fake favorite schools and returns it to the caller.
        /// </summary>
        private List<FavoriteSchoolViewModel> GetFakeFavoriteSchools(string userId)
        {
            // EXEMPLE: retorna llista fake. Substitueix per crida a service real si existeix
            return new List<FavoriteSchoolViewModel>
            {
                new FavoriteSchoolViewModel { Id = 1, Name = "Escola Pia de Sarrià", Municipality = "Barcelona", Url = "#" },
                new FavoriteSchoolViewModel { Id = 2, Name = "Institut Montserrat", Municipality = "Barcelona", Url = "#" }
            };
        }
    }
}
