using RealEstate.DTOs.Properties.Leases;
using RealEstate.Entities.Persons;
using RealEstate.Entities.Properties.Leases;
using RealEstate.Repositories.Properties.Leases;

namespace RealEstate.Services.Properties.Leases;

interface ILeaseService
{
    Task<IEnumerable<Lease>> GetListAsync();
    Task<Lease> GetAsync(Guid id);
    Task<Lease> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class LeaseService(LeaseRepository repository) : ILeaseService
{
    private readonly LeaseRepository _repository = repository;

    public async Task<IEnumerable<Lease>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<Lease> GetAsync(Guid id)
    {
        var lease = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(lease);

        return lease;
    }

    public async Task<Lease> AddAsync(CreateDTO createDTO)
    {
        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new Lease
        {
            MonthlyRent = createDTO.MonthlyRent,
            DepositAmount = createDTO.DepositAmount,
            EndTime = createDTO.EndTime,
            EndDate = createDTO.EndDate,
            PropertyId = createDTO.PropertyId,
            OwnerId = createDTO.OwnerId,
            TenantId = createDTO.TenantId
        }).ConfigureAwait(false); ;
    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO)
    {
        var lease = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(lease);

        ArgumentNullException.ThrowIfNull(updateDTO);

        lease.MonthlyRent = updateDTO.MonthlyRent != lease.MonthlyRent ? updateDTO.MonthlyRent : lease.MonthlyRent;
        lease.DepositAmount = updateDTO.DepositAmount != lease.DepositAmount ? updateDTO.DepositAmount : lease.DepositAmount;
        lease.EndTime = updateDTO.EndTime !=  lease.EndTime ? updateDTO.EndTime : lease.EndTime; 
        lease.EndDate = updateDTO.EndDate !=  lease.EndDate ? updateDTO.EndDate : lease.EndDate; 
        lease.PropertyId = updateDTO.PropertyId != lease.PropertyId ? updateDTO.PropertyId : lease.PropertyId;
        lease.OwnerId = updateDTO.OwnerId != lease.OwnerId ? updateDTO.OwnerId : lease.OwnerId;
        lease.TenantId = updateDTO.TenantId != lease.TenantId ? updateDTO.TenantId : lease.TenantId;

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
