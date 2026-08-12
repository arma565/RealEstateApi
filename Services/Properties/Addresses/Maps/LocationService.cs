using RealEstate.DTOs.Properties.Addresses.Map;
using RealEstate.Entities.Persons;
using RealEstate.Entities.Properties.Addresses.Map;
using RealEstate.Repositories.Properties.Addresses.Maps;

namespace RealEstate.Services.Properties.Addresses.Maps;

interface ILocationService
{
    Task<IEnumerable<PropertyLocation>> GetListAsync();
    Task<PropertyLocation> GetAsync(Guid id);
    Task<PropertyLocation> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(UpdateDTO updateDTO, Guid id);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class LocationService(LocationRepository repository) : ILocationService
{
    private readonly LocationRepository _repository = repository;

    public async Task<IEnumerable<PropertyLocation>> GetListAsync() =>
     await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyLocation> GetAsync(Guid id) { 
        var location = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(location);
        return location;
    }
    
    public async Task<PropertyLocation> AddAsync(CreateDTO createDTO)
    {
        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new PropertyLocation
        {
            Latitude = createDTO.Latitude,
            Longitude = createDTO.Longitude,
            PropertyId = createDTO.PropertyId
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(UpdateDTO updateDTO, Guid id)
    {
        ArgumentNullException.ThrowIfNull(updateDTO);

        var location = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(location);

        location.Latitude = updateDTO.Latitude != location.Latitude ? updateDTO.Latitude : location.Latitude;
        location.Longitude = updateDTO.Longitude != location.Longitude ? updateDTO.Longitude : location.Longitude;
        location.PropertyId = updateDTO.PropertyId != location.PropertyId ? updateDTO.PropertyId : location.PropertyId;

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
