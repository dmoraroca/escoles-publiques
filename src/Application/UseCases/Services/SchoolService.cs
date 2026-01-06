using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.DomainExceptions;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;

/// <summary>
/// Service for managing schools, including retrieval, creation, update, and deletion of school records.
/// </summary>
public class SchoolService : ISchoolService
{
    private readonly ISchoolRepository _schoolRepository;
    private readonly ILogger<SchoolService> _logger;

    /// <summary>
    /// Initializes a new instance of the SchoolService class.
    /// </summary>
    /// <param name="schoolRepository">Repository for school data access.</param>
    /// <param name="logger">Logger instance.</param>
    public SchoolService(ISchoolRepository schoolRepository, ILogger<SchoolService> logger)
    {
        _schoolRepository = schoolRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all schools asynchronously.
    /// </summary>
    /// <returns>Enumerable of all schools.</returns>
    public async Task<IEnumerable<School>> GetAllSchoolsAsync()
    {
        _logger.LogInformation("Obtenint totes les escoles");
        return await _schoolRepository.GetAllAsync();
    }

    /// <summary>
    /// Retrieves a school by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">School identifier.</param>
    /// <returns>The school if found; otherwise, throws NotFoundException.</returns>
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
    /// Retrieves a school by its code asynchronously.
    /// </summary>
    /// <param name="code">School code.</param>
    /// <returns>The school if found; otherwise, null.</returns>
    public async Task<School?> GetSchoolByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ValidationException("Code", "El codi de l'escola no pot estar buit");
        }
        
        _logger.LogInformation("Obtenint escola amb codi: {Code}", code);
        return await _schoolRepository.GetByCodeAsync(code);
    }

    public async Task<School> CreateSchoolAsync(School school)
    {
        if (string.IsNullOrWhiteSpace(school.Code))
        {
            throw new ValidationException("Code", "El codi de l'escola és obligatori");
        }
        
        if (string.IsNullOrWhiteSpace(school.Name))
        {
            throw new ValidationException("Name", "El nom de l'escola és obligatori");
        }
        
        var existingSchool = await _schoolRepository.GetByCodeAsync(school.Code);
        if (existingSchool != null)
        {
            _logger.LogWarning("Intent de crear escola amb codi duplicat: {Code}", school.Code);
            throw new DuplicateEntityException("School", "Code", school.Code);
        }

        school.CreatedAt = DateTime.UtcNow;
        _logger.LogInformation("Creant nova escola: {Name} ({Code})", school.Name, school.Code);
        return await _schoolRepository.AddAsync(school);
    }

    public async Task UpdateSchoolAsync(School school)
    {
        var existingSchool = await _schoolRepository.GetByIdAsync(school.Id);
        if (existingSchool == null)
        {
            throw new NotFoundException("School", school.Id);
        }
        
        _logger.LogInformation("Actualitzant escola amb Id: {Id}", school.Id);
        await _schoolRepository.UpdateAsync(school);
    }

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
