using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;

public class SchoolService : ISchoolService
{
    private readonly ISchoolRepository _schoolRepository;
    private readonly ILogger<SchoolService> _logger;

    public SchoolService(ISchoolRepository schoolRepository, ILogger<SchoolService> logger)
    {
        _schoolRepository = schoolRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<School>> GetAllSchoolsAsync()
    {
        try
        {
            return await _schoolRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtenir totes les escoles");
            throw;
        }
    }

    public async Task<School?> GetSchoolByIdAsync(long id)
    {
        return await _schoolRepository.GetByIdAsync(id);
    }

    public async Task<School?> GetSchoolByCodeAsync(string code)
    {
        return await _schoolRepository.GetByCodeAsync(code);
    }

    public async Task<School> CreateSchoolAsync(School school)
    {
        // Validacions de negoci
        var existingSchool = await _schoolRepository.GetByCodeAsync(school.Code);
        if (existingSchool != null)
        {
            throw new InvalidOperationException($"Ja existeix una escola amb el codi {school.Code}");
        }

        school.CreatedAt = DateTime.UtcNow;
        return await _schoolRepository.AddAsync(school);
    }

    public async Task UpdateSchoolAsync(School school)
    {
        await _schoolRepository.UpdateAsync(school);
    }

    public async Task DeleteSchoolAsync(long id)
    {
        await _schoolRepository.DeleteAsync(id);
    }
}
