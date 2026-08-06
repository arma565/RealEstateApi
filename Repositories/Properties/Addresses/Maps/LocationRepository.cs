using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Addresses.Map;

namespace RealEstate.Repositories.Properties.Addresses.Maps;

interface ILocationRepository
{
    Task<IEnumerable<PropertyLocation>> GetListAsync();
    Task<PropertyLocation?> GetAsync(Guid id);
    Task<PropertyLocation> AddAsync(PropertyLocation propertyLocation);
    Task UpdateAsync(PropertyLocation propertyLocation);
    Task DeleteAsync(PropertyLocation propertyLocation);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class LocationRepository(AppDbContext context) : ILocationRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PropertyLocation>> GetListAsync() =>
      await _context
        .Locations
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<PropertyLocation?> GetAsync(Guid id) =>
       await _context
         .Locations
         .AsNoTracking()
         .SingleOrDefaultAsync(location => location.Id == id)
         .ConfigureAwait(false);

    public async Task<PropertyLocation> AddAsync(PropertyLocation propertyLocation)
    {
        await _context.Locations.AddAsync(propertyLocation).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyLocation;
    }

    public async Task UpdateAsync(PropertyLocation propertyLocation)
    {
        _context.Locations.Update(propertyLocation);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(PropertyLocation propertyLocation)
    {
        _context.Locations.Remove(propertyLocation);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Locations.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
