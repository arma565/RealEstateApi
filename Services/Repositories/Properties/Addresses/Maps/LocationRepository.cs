using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Properties.Addresses.Map;

namespace RealEstate.Services.Repositories.Properties.Addresses.Maps;

interface ILocationRepository
{
    Task<IEnumerable<PropertyLocation>> GetListAsync();
    Task<PropertyLocation?> GetAsync(Guid id);
    Task AddAsync(PropertyLocation location);
    Task UpdateAsync(PropertyLocation location);
    Task DeleteAsync(Guid id);
    Task<bool> IsLocationExistAsync(Guid id);
}

#pragma warning disable CA1515
public class LocationRepository(AppDbContext context) : ILocationRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PropertyLocation>> GetListAsync() =>
      await _context
        .Locations
        .AsNoTracking()
        .Include(location => location.Property)
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<PropertyLocation?> GetAsync(Guid id) =>
       await _context
         .Locations
         .AsNoTracking()
         .Include(location => location.Property)
         .SingleOrDefaultAsync(location => location.Id == id)
         .ConfigureAwait(false);

    public async Task AddAsync(PropertyLocation location)
    {
        await _context.Locations.AddAsync(location).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(PropertyLocation location)
    {
        _context.Locations.Update(location);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var location = await _context.Locations.FindAsync(id).ConfigureAwait(false);

        if (location == null)
            ArgumentNullException.ThrowIfNull(location);

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<bool> IsLocationExistAsync(Guid id) =>
        await _context.Locations.AsNoTracking().AnyAsync(location => location.Id == id).ConfigureAwait(false);
}
