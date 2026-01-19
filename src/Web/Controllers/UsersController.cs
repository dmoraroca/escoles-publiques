using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

[Authorize]
public class UsersController : BaseController
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger) : base(logger)
    {
        _userRepository = userRepository;
    }

    // Llista només els usuaris que tenen relació 1:1 amb Student
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
