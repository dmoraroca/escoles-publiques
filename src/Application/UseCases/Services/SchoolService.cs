using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.DomainExceptions;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;
/// <summary>
/// Implements application logic for school operations.
/// </summary>
public class SchoolService : ISchoolService
{
    private readonly ISchoolRepository _schoolRepository;
    private readonly ILogger<SchoolService> _logger;
            /// <summary>
            /// Initializes a new instance of the SchoolService class with its required dependencies.
            /// </summary>
            public SchoolService(ISchoolRepository schoolRepository, ILogger<SchoolService> logger)
    {
        _schoolRepository = schoolRepository;
        _logger = logger;
    }
            /// <summary>
            /// Retrieves all schools async and returns it to the caller.
            /// </summary>
            public async Task<IEnumerable<School>> GetAllSchoolsAsync()
    {
        _logger.LogInformation("Obtenint totes les escoles");
        return await _schoolRepository.GetAllAsync();
    }
            /// <summary>
            /// Retrieves school by id async and returns it to the caller.
            /// </summary>
            public async Task<School?> GetSchoolByIdAsync(long id)
    {
        _logger.LogInformation("Obtenint escola amb Id: {Id}", id);
        var school = await _schoolRepository.GetByIdAsync(id);

        if (school == null)
        {
            _logger.LogWarning("Escola amb Id {Id} no trobada", id);
            throw new NotFoundException("School", id);
        }

        return school;
    }
            /// <summary>
            /// Retrieves school by code async and returns it to the caller.
            /// </summary>
            public async Task<School?> GetSchoolByCodeAsync(string code)
    {
        var schoolCode = SchoolCode.Create(code);

        _logger.LogInformation("Obtenint escola amb codi: {Code}", schoolCode.Value);
        return await _schoolRepository.GetByCodeAsync(schoolCode.Value);
    }
            /// <summary>
            /// Creates school async by applying the required business rules.
            /// </summary>
            public async Task<School> CreateSchoolAsync(School school)
    {
        if (string.IsNullOrWhiteSpace(school.Name))
        {
            throw new ValidationException("Name", "El nom de l'escola és obligatori");
        }

        var schoolCode = SchoolCode.Create(school.Code);
        school.Code = schoolCode.Value;

        var existingSchool = await _schoolRepository.GetByCodeAsync(schoolCode.Value);
        if (existingSchool != null)
        {
            _logger.LogWarning("Intent de crear escola amb codi duplicat: {Code}", schoolCode.Value);
            throw new DuplicateEntityException("School", "Code", schoolCode.Value);
        }

        school.CreatedAt = DateTime.UtcNow;
        _logger.LogInformation("Creant nova escola: {Name} ({Code})", school.Name, school.Code);
        return await _schoolRepository.AddAsync(school);
    }
            /// <summary>
            /// Updates school async with the data received in the request.
            /// </summary>
            public async Task UpdateSchoolAsync(School school)
    {
        var existingSchool = await _schoolRepository.GetByIdAsync(school.Id);
        if (existingSchool == null)
        {
            throw new NotFoundException("School", school.Id);
        }

        if (string.IsNullOrWhiteSpace(school.Name))
        {
            throw new ValidationException("Name", "El nom de l'escola és obligatori");
        }

        school.Code = SchoolCode.Create(school.Code).Value;

        _logger.LogInformation("Actualitzant escola amb Id: {Id}", school.Id);
        await _schoolRepository.UpdateAsync(school);
    }
            /// <summary>
            /// Deletes school async from the system in a controlled manner.
            /// </summary>
            public async Task DeleteSchoolAsync(long id)
    {
        var school = await _schoolRepository.GetByIdAsync(id);
        if (school == null)
        {
            throw new NotFoundException("School", id);
        }

        _logger.LogInformation("Eliminant escola amb Id: {Id}", id);
        await _schoolRepository.DeleteAsync(id);
    }
}
