using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Documents;

namespace RealEstate.Repositories.Properties.Documents;

interface IDeedRepository
{
    Task<IEnumerable<PropertyDeed>> GetListAsync();
    Task<PropertyDeed?> GetAsync(Guid id);
    Task<PropertyDeed> AddAsync(PropertyDeed propertyDeed);
    Task UpdateAsync(PropertyDeed propertyDeed);
    Task DeleteAsync(PropertyDeed propertyDeed);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class DeedRepository(AppDbContext context) : IDeedRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PropertyDeed>> GetListAsync() =>
     await _context
        .PropertyDeeds
        .AsNoTracking()
        .Include(deed => deed.Image)
        .Include(deed => deed.Property)
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<PropertyDeed?> GetAsync(Guid id) =>
    await _context
       .PropertyDeeds
       .AsNoTracking()
       .Include(deed => deed.Image)
       .Include(deed => deed.Property)
       .SingleOrDefaultAsync(deed => deed.Id == id)
       .ConfigureAwait(false);

    public async Task<PropertyDeed> AddAsync(PropertyDeed propertyDeed)
    {
        await _context.PropertyDeeds.AddAsync(propertyDeed).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyDeed;
    }

    public async Task UpdateAsync(PropertyDeed propertyDeed)
    {
        _context.PropertyDeeds.Update(propertyDeed);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(PropertyDeed propertyDeed)
    {
        _context.PropertyDeeds.Remove(propertyDeed);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.PropertyDeeds.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
