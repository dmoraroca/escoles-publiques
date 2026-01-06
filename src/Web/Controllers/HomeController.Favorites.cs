using Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Web.Controllers
{
    public partial class HomeController : BaseController
    {
        private List<FavoriteSchoolViewModel> GetFakeFavoriteSchools(string userId)
        {
            // EXEMPLE: retorna llista fake. Substitueix per crida a servei real si existeix
            return new List<FavoriteSchoolViewModel>
            {
                new FavoriteSchoolViewModel { Id = 1, Name = "Escola Pia de Sarrià", Municipality = "Barcelona", Url = "#" },
                new FavoriteSchoolViewModel { Id = 2, Name = "Institut Montserrat", Municipality = "Barcelona", Url = "#" }
            };
        }
    }
}
