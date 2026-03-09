using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.DomainExceptions;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;
/// <summary>
/// Implements application logic for annual fee operations.
/// </summary>
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
        /// <summary>
        /// Retrieves all annual fees async and returns it to the caller.
        /// </summary>
        public async Task<IEnumerable<AnnualFee>> GetAllAnnualFeesAsync()
    {
        _logger.LogInformation("Obtenint totes les quotes");
        return await _annualFeeRepository.GetAllAsync();
    }
        /// <summary>
        /// Retrieves annual fee by id async and returns it to the caller.
        /// </summary>
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
        /// <summary>
        /// Retrieves annual fees by enrollment id async and returns it to the caller.
        /// </summary>
        public async Task<IEnumerable<AnnualFee>> GetAnnualFeesByEnrollmentIdAsync(long enrollmentId)
    {
        _logger.LogInformation("Obtenint quotes de la inscripció amb Id: {EnrollmentId}", enrollmentId);
        return await _annualFeeRepository.GetByEnrollmentIdAsync(enrollmentId);
    }
        /// <summary>
        /// Creates annual fee async by applying the required business rules.
        /// </summary>
        public async Task<AnnualFee> CreateAnnualFeeAsync(AnnualFee annualFee)
    {
        annualFee.Amount = MoneyAmount.Create(annualFee.Amount).Value;

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
        /// <summary>
        /// Updates annual fee async with the data received in the request.
        /// </summary>
        public async Task UpdateAnnualFeeAsync(AnnualFee annualFee)
    {
        var existingFee = await _annualFeeRepository.GetByIdAsync(annualFee.Id);
        if (existingFee == null)
        {
            throw new NotFoundException("AnnualFee", annualFee.Id);
        }

        annualFee.Amount = MoneyAmount.Create(annualFee.Amount).Value;

        _logger.LogInformation("Actualitzant quota amb Id: {Id}", annualFee.Id);
        await _annualFeeRepository.UpdateAsync(annualFee);
    }
        /// <summary>
        /// Deletes annual fee async from the system in a controlled manner.
        /// </summary>
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
