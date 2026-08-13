using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Leases;

namespace RealEstate.Repositories.Properties.Leases;

interface ILeaseRepository
{
    Task<IEnumerable<Lease>> GetListAsync();
    Task<Lease?> GetAsync(Guid id);
    Task<Lease> AddAsync(Lease lease);
    Task UpdateAsync(Lease lease);
    Task DeleteAsync(Lease lease);
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
        .Include(lease => lease.Payments)
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<Lease?> GetAsync(Guid id) =>
    await _context
       .Leases
       .AsNoTracking()
       .Include(lease => lease.Payments)
       .SingleOrDefaultAsync(lease => lease.Id == id)
       .ConfigureAwait(false);

    public async Task<Lease> AddAsync(Lease lease)
    {
        await _context.Leases.AddAsync(lease).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return lease;
    }

    public async Task UpdateAsync(Lease lease)
    {
        _context.Leases.Update(lease);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Lease lease)
    {
        _context.Leases.Remove(lease);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Leases.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
