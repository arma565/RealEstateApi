using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Documents;

namespace RealEstate.Repositories.Properties.Documents;

#pragma warning disable CA1515
public class PropertyDeedRepository<TPropertyDeed>(AppDbContext context) : BaseRepository<PropertyDeed>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<PropertyDeed>> GetListAsync() =>
     await _context
        .PropertyDeeds
        .AsNoTracking()
        .Include(propertyDeed => propertyDeed.PropertyDeedImages)
        .ToListAsync()
        .ConfigureAwait(false);

    public override async Task<PropertyDeed?> GetAsync(Guid id) =>
    await _context
       .PropertyDeeds
       .AsNoTracking()
       .Include(propertyDeed => propertyDeed.PropertyDeedImages)
       .SingleOrDefaultAsync(deed => deed.Id == id)
       .ConfigureAwait(false);

    public override async Task<PropertyDeed> AddAsync(PropertyDeed propertyDeed)
    {
        await _context.PropertyDeeds.AddAsync(propertyDeed).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyDeed;
    }

    public override async Task UpdateAsync(PropertyDeed propertyDeed)
    {
        _context.PropertyDeeds.Update(propertyDeed);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(PropertyDeed propertyDeed)
    {
        _context.PropertyDeeds.Remove(propertyDeed);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.PropertyDeeds.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
