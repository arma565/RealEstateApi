using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Leases;

namespace RealEstate.Repositories.Properties.Leases;

#pragma warning disable CA1515
public class LeaseRepository<TLease>(AppDbContext context) : BaseRepository<Lease>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<Lease>> GetListAsync() =>
     await _context
        .Leases
        .AsNoTracking()
        .Include(lease => lease.Payments)
        .ToListAsync()
        .ConfigureAwait(false);

    public override async Task<Lease?> GetAsync(Guid id) =>
    await _context
       .Leases
       .AsNoTracking()
       .Include(lease => lease.Payments)
       .SingleOrDefaultAsync(lease => lease.Id == id)
       .ConfigureAwait(false);

    public override async Task<Lease> AddAsync(Lease lease)
    {
        await _context.Leases.AddAsync(lease).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return lease;
    }

    public override async Task UpdateAsync(Lease lease)
    {
        _context.Leases.Update(lease);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(Lease lease)
    {
        _context.Leases.Remove(lease);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.Leases.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
