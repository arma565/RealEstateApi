using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Persons.Tenants;

namespace RealEstate.Repositories.Tenants;

interface ITenantRepository
{
    Task<IEnumerable<Tenant>> GetListAsync();
    Task<Tenant?> GetAsync(Guid id);
    Task<Tenant> AddAsync(Tenant tenant);
    Task UpdateAsync(Tenant tenant);
    Task DeleteAsync(Tenant tenant);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class TenantRepository(AppDbContext context) : ITenantRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Tenant>> GetListAsync() =>
        await _context
            .Tenants
            .Include(tenant => tenant.Leases)
            .AsNoTracking()
            .OrderByDescending(per => per.Id)
            .ToListAsync().ConfigureAwait(false);

    public async Task<Tenant?> GetAsync(Guid id) =>
         await _context
            .Tenants
            .Include(tenant => tenant.Leases)
            .AsNoTracking()
            .SingleOrDefaultAsync(tenant => tenant.Id == id)
            .ConfigureAwait(false);

    public async Task<Tenant> AddAsync(Tenant tenant)
    {
        await _context.Tenants.AddAsync(tenant).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return tenant;
    }

    public async Task UpdateAsync(Tenant tenant)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Tenant tenant)
    {
        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Tenants.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}



