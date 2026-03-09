using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Microsoft.Extensions.Localization;

namespace Web.Controllers;

/// <summary>
/// Exposes HTTP endpoints to manage users workflows.
/// </summary>
[Authorize]
public class UsersController : BaseController
{
    private readonly IUserRepository _userRepository;
    /// <summary>
    /// Initializes a new instance of the UsersController class with its required dependencies.
    /// </summary>
    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger, IStringLocalizer<BaseController>? localizer = null) : base(logger, localizer)
    {
        _userRepository = userRepository;
    }

    // Llista només els usuaris que tenen relació 1:1 amb Student
    /// <summary>
    /// Executes the index operation as part of this component.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var viewModels = users
                .Where(u => u.Student != null)
                .Select(u => new UserViewModel
                {
                    Id = (int)u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    BirthDate = u.BirthDate.HasValue ? u.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    StudentId = u.Student != null ? (int?)u.Student.Id : null
                })
                .ToList();

            return View(viewModels);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error carregant llistat d'usuaris");
            SetErrorMessage("Error carregant usuaris. Si us plau, intenta-ho de nou.");
            return View(new List<UserViewModel>());
        }
    }
}
