using RealEstate.Entities.Properties.Leases;
using RealEstate.Repositories.Properties.Leases;

namespace RealEstate.Services.Properties.Leases;

interface ILeaseService
{
    Task<IEnumerable<Lease>> GetListAsync();
    Task<Lease?> GetAsync(Guid id);
    Task<Lease> AddAsync(LeaseDTO leaseDTO);
    Task UpdateAsync(Guid id, LeaseDTO leaseDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class LeaseService(LeaseRepository repository) : ILeaseService
{
    private readonly LeaseRepository _repository = repository;

    public async Task<IEnumerable<Lease>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<Lease?> GetAsync(Guid id) =>
    await _repository.GetAsync(id).ConfigureAwait(false);

    public async Task<Lease> AddAsync(LeaseDTO leaseDTO)
    {
        ArgumentNullException.ThrowIfNull(leaseDTO);

        var lease = new Lease
        {
            MonthlyRent = leaseDTO.MonthlyRent,
            DepositAmount = leaseDTO.DepositAmount,
            StartTime = leaseDTO.StartTime ?? TimeOnly.MinValue,
            EndTime = leaseDTO.EndTime,
            StartDate = leaseDTO.StartDate,
            EndDate = leaseDTO.EndDate,
            PropertyId = leaseDTO.PropertyId
        };

        return await _repository.AddAsync(lease).ConfigureAwait(false); ;
    }

    public async Task UpdateAsync(Guid id, LeaseDTO leaseDTO)
    {
        ArgumentNullException.ThrowIfNull(leaseDTO);

        var lease = new Lease
        {
            Id = id,
            MonthlyRent = leaseDTO.MonthlyRent,
            DepositAmount = leaseDTO.DepositAmount,
            StartTime = leaseDTO.StartTime ?? TimeOnly.MinValue,
            EndTime = leaseDTO.EndTime,
            StartDate = leaseDTO.StartDate,
            EndDate = leaseDTO.EndDate,
            PropertyId = leaseDTO.PropertyId
        };

        await _repository.UpdateAsync(lease).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var lease = await GetAsync(id).ConfigureAwait(false);
        
        ArgumentNullException.ThrowIfNull(lease);

        await _repository.DeleteAsync(lease).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
         await _repository.DeleteAllAsync().ConfigureAwait(false);
}
