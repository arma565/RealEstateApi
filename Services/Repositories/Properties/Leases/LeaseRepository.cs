using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Properties.Leases;

namespace RealEstate.Services.Repositories.Properties.Leases;

interface ILeaseRepository
{
    Task<IEnumerable<Lease>> GetListAsync();
    Task<Lease?> GetAsync(Guid id);
    Task<Lease> AddAsync(LeaseDTO leaseDTO);
    Task UpdateAsync(Guid id , LeaseDTO leaseDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class LeaseRepository(AppDbContext context) : ILeaseRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Lease>> GetListAsync() =>
     await _context
        .Leases
        .AsNoTracking()
        .Include(lease => lease.Persons)
        .Include(lease => lease.Payments)
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<Lease?> GetAsync(Guid id) =>
    await _context
       .Leases
       .AsNoTracking()
       .Include(lease => lease.Persons)
       .Include(lease => lease.Payments)
       .SingleOrDefaultAsync(lease => lease.Id == id)
       .ConfigureAwait(false);

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

        await _context.Leases.AddAsync(lease).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return lease;
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

        _context.Leases.Update(lease);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var lease = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(lease);

        _context.Leases.Remove(lease);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Leases.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
