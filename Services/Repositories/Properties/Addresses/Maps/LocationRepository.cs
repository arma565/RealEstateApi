using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Properties.Addresses.Map;

namespace RealEstate.Services.Repositories.Properties.Addresses.Maps;

interface ILocationRepository
{
    Task<IEnumerable<PropertyLocation>> GetListAsync();
    Task<PropertyLocation?> GetAsync(Guid id);
    Task<PropertyLocation> AddAsync(PropertyLocationDTO propertyLocationDTO);
    Task UpdateAsync(PropertyLocationDTO propertyLocationDTO, Guid id);
    Task DeleteAsync(Guid id);
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

    public async Task<PropertyLocation> AddAsync(PropertyLocationDTO propertyLocationDTO)
    {
        ArgumentNullException.ThrowIfNull(propertyLocationDTO);

        var location = new PropertyLocation
        {
            Latitude = propertyLocationDTO.Latitude,
            Longitude = propertyLocationDTO.Longitude,
            PropertyId = propertyLocationDTO.PropertyId
        };

        await _context.Locations.AddAsync(location).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return location;
    }

    public async Task UpdateAsync(PropertyLocationDTO propertyLocationDTO , Guid id)
    {
        ArgumentNullException.ThrowIfNull(propertyLocationDTO);

        var location = new PropertyLocation
        {
            Id = id,
            Latitude = propertyLocationDTO.Latitude,
            Longitude = propertyLocationDTO.Longitude,
            PropertyId = propertyLocationDTO.PropertyId
        };

        _context.Locations.Update(location);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var location = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(location);

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Locations.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
