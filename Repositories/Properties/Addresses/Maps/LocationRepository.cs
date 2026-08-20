using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Addresses.Map;

namespace RealEstate.Repositories.Properties.Addresses.Maps;

#pragma warning disable CA1515
public class LocationRepository<TPropertyLocation>(AppDbContext context) : BaseRepository<PropertyLocation>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<PropertyLocation>> GetListAsync() =>
      await _context
        .Locations
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);

    public override async Task<PropertyLocation?> GetAsync(Guid id) =>
       await _context
         .Locations
         .AsNoTracking()
         .SingleOrDefaultAsync(location => location.Id == id)
         .ConfigureAwait(false);

    public override async Task<PropertyLocation> AddAsync(PropertyLocation propertyLocation)
    {
        await _context.Locations.AddAsync(propertyLocation).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyLocation;
    }

    public override async Task UpdateAsync(PropertyLocation propertyLocation)
    {
        _context.Locations.Update(propertyLocation);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(PropertyLocation propertyLocation)
    {
        _context.Locations.Remove(propertyLocation);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.Locations.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
