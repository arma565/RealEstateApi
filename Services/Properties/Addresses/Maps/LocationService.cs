using RealEstate.Entities.Properties.Addresses.Map;
using RealEstate.Repositories.Properties.Addresses.Maps;

namespace RealEstate.Services.Properties.Addresses.Maps;

interface ILocationService
{
    Task<IEnumerable<PropertyLocation>> GetListAsync();
    Task<PropertyLocation?> GetAsync(Guid id);
    Task<PropertyLocation> AddAsync(PropertyLocationDTO propertyLocationDTO);
    Task UpdateAsync(PropertyLocationDTO propertyLocationDTO, Guid id);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class LocationService(LocationRepository repository) : ILocationService
{
    private readonly LocationRepository _repository = repository;

    public async Task<IEnumerable<PropertyLocation>> GetListAsync() =>
     await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyLocation?> GetAsync(Guid id) =>
    await _repository.GetAsync(id).ConfigureAwait(false);

    public async Task<PropertyLocation> AddAsync(PropertyLocationDTO propertyLocationDTO)
    {
        ArgumentNullException.ThrowIfNull(propertyLocationDTO);

        var location = new PropertyLocation
        {
            Latitude = propertyLocationDTO.Latitude,
            Longitude = propertyLocationDTO.Longitude,
            PropertyId = propertyLocationDTO.PropertyId
        };

        return await _repository.AddAsync(location).ConfigureAwait(false);
    }

    public async Task UpdateAsync(PropertyLocationDTO propertyLocationDTO, Guid id)
    {
        ArgumentNullException.ThrowIfNull(propertyLocationDTO);

        var location = new PropertyLocation
        {
            Id = id,
            Latitude = propertyLocationDTO.Latitude,
            Longitude = propertyLocationDTO.Longitude,
            PropertyId = propertyLocationDTO.PropertyId
        };

        await _repository.UpdateAsync(location).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var location = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(location);

        await _repository.DeleteAsync(location).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
        await _repository.DeleteAllAsync().ConfigureAwait(false);

}
