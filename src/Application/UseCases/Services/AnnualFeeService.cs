using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.DomainExceptions;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;

public class AnnualFeeService : IAnnualFeeService
{
    private readonly IAnnualFeeRepository _annualFeeRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILogger<AnnualFeeService> _logger;

    public AnnualFeeService(
        IAnnualFeeRepository annualFeeRepository,
        IEnrollmentRepository enrollmentRepository,
        ILogger<AnnualFeeService> logger)
    {
        _annualFeeRepository = annualFeeRepository;
        _enrollmentRepository = enrollmentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AnnualFee>> GetAllAnnualFeesAsync()
    {
        _logger.LogInformation("Obtenint totes les quotes");
        return await _annualFeeRepository.GetAllAsync();
    }

    public async Task<AnnualFee?> GetAnnualFeeByIdAsync(long id)
    {
        _logger.LogInformation("Obtenint quota amb Id: {Id}", id);
        var annualFee = await _annualFeeRepository.GetByIdAsync(id);
        
        if (annualFee == null)
        {
            _logger.LogWarning("Quota amb Id {Id} no trobada", id);
            throw new NotFoundException("AnnualFee", id);
        }
        
        return annualFee;
    }

    public async Task<IEnumerable<AnnualFee>> GetAnnualFeesByEnrollmentIdAsync(long enrollmentId)
    {
        _logger.LogInformation("Obtenint quotes de la inscripció amb Id: {EnrollmentId}", enrollmentId);
        return await _annualFeeRepository.GetByEnrollmentIdAsync(enrollmentId);
    }

    public async Task<AnnualFee> CreateAnnualFeeAsync(AnnualFee annualFee)
    {
        // Validacions de negoci
        if (annualFee.Amount <= 0)
        {
            throw new ValidationException("Amount", "L'import ha de ser superior a 0");
        }
        
        if (annualFee.EnrollmentId <= 0)
        {
            throw new ValidationException("EnrollmentId", "L'ID de la inscripció és obligatori");
        }
        
        var enrollment = await _enrollmentRepository.GetByIdAsync(annualFee.EnrollmentId);
        if (enrollment == null)
        {
            throw new NotFoundException("Enrollment", annualFee.EnrollmentId);
        }

        _logger.LogInformation("Creant nova quota per la inscripció {EnrollmentId} - Import: {Amount}", 
            annualFee.EnrollmentId, annualFee.Amount);
        return await _annualFeeRepository.AddAsync(annualFee);
    }

    public async Task UpdateAnnualFeeAsync(AnnualFee annualFee)
    {
        var existingFee = await _annualFeeRepository.GetByIdAsync(annualFee.Id);
        if (existingFee == null)
        {
            throw new NotFoundException("AnnualFee", annualFee.Id);
        }
        
        _logger.LogInformation("Actualitzant quota amb Id: {Id}", annualFee.Id);
        await _annualFeeRepository.UpdateAsync(annualFee);
    }

    public async Task DeleteAnnualFeeAsync(long id)
    {
        var annualFee = await _annualFeeRepository.GetByIdAsync(id);
        if (annualFee == null)
        {
            throw new NotFoundException("AnnualFee", id);
        }
        
        _logger.LogInformation("Eliminant quota amb Id: {Id}", id);
        await _annualFeeRepository.DeleteAsync(id);
    }
}
