using RealEstate.DTOs.Properties.Documents;
using RealEstate.Entities.Properties.Documents;
using RealEstate.Repositories.Properties.Documents;

namespace RealEstate.Services.Properties.Documents;

interface IDeedService
{
    Task<IEnumerable<PropertyDeed>> GetListAsync();
    Task<PropertyDeed> GetAsync(Guid id);
    Task<PropertyDeed> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PropertyDeedService(PropertyDeedRepository repository) : IDeedService
{
    private readonly PropertyDeedRepository _repository = repository;

    public async Task<IEnumerable<PropertyDeed>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyDeed> GetAsync(Guid id) {
        var propertyDeed = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyDeed);

        return propertyDeed;
    }
        
    public async Task<PropertyDeed> AddAsync(CreateDTO createDTO)
    {
        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new PropertyDeed
        {
            DeedNumber = createDTO.DeedNumber,
            RegistryNumber = createDTO.RegistryNumber,
            IssuedBy = createDTO.IssuedBy,
            PropertyId = createDTO.PropertyId
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO)
    {
        var propertyDeed = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyDeed);

        ArgumentNullException.ThrowIfNull(updateDTO);

        propertyDeed.DeedNumber = string.IsNullOrEmpty(updateDTO.DeedNumber) ? propertyDeed.DeedNumber : updateDTO.DeedNumber;
        propertyDeed.RegistryNumber = string.IsNullOrEmpty(updateDTO.RegistryNumber) ? propertyDeed.RegistryNumber : updateDTO.RegistryNumber;
        propertyDeed.IssuedBy = string.IsNullOrEmpty(updateDTO.IssuedBy) ? propertyDeed.IssuedBy : updateDTO.IssuedBy;
        propertyDeed.PropertyId = updateDTO.PropertyId != propertyDeed.PropertyId ? updateDTO.PropertyId : propertyDeed.PropertyId;

        await _repository.UpdateAsync(propertyDeed).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var propertyDeed = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyDeed);

        await _repository.DeleteAsync(propertyDeed).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() => 
        await _repository.DeleteAllAsync().ConfigureAwait(false);

}
