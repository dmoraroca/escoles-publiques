using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Services;

public class AnnualFeeService : IAnnualFeeService
{
    private readonly IAnnualFeeRepository _annualFeeRepository;
    private readonly ILogger<AnnualFeeService> _logger;

    public AnnualFeeService(IAnnualFeeRepository annualFeeRepository, ILogger<AnnualFeeService> logger)
    {
        _annualFeeRepository = annualFeeRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<AnnualFee>> GetAllAnnualFeesAsync()
    {
        try
        {
            return await _annualFeeRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtenir totes les quotes");
            throw;
        }
    }

    public async Task<AnnualFee?> GetAnnualFeeByIdAsync(long id)
    {
        return await _annualFeeRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<AnnualFee>> GetAnnualFeesByEnrollmentIdAsync(long enrollmentId)
    {
        return await _annualFeeRepository.GetByEnrollmentIdAsync(enrollmentId);
    }

    public async Task<AnnualFee> CreateAnnualFeeAsync(AnnualFee annualFee)
    {
        return await _annualFeeRepository.AddAsync(annualFee);
    }

    public async Task UpdateAnnualFeeAsync(AnnualFee annualFee)
    {
        await _annualFeeRepository.UpdateAsync(annualFee);
    }

    public async Task DeleteAnnualFeeAsync(long id)
    {
        await _annualFeeRepository.DeleteAsync(id);
    }
}
