using RealEstate.DTOs.Properties.Documents;
using RealEstate.Entities.Properties.Documents;
using RealEstate.Repositories.Properties.Documents;

namespace RealEstate.Services.Properties.Documents;

interface IDeedService
{
    Task<IEnumerable<PropertyDeed>> GetListAsync();
    Task<PropertyDeed?> GetAsync(Guid id);
    Task<PropertyDeed> AddAsync(PropertyDeedDTO propertyDeedDTO);
    Task UpdateAsync(Guid id, PropertyDeedDTO propertyDeedDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class DeedService(DeedRepository repository) : IDeedService
{
    private readonly DeedRepository _repository = repository;

    public async Task<IEnumerable<PropertyDeed>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyDeed?> GetAsync(Guid id) =>
        await _repository.GetAsync(id).ConfigureAwait(false);

    public async Task<PropertyDeed> AddAsync(PropertyDeedDTO propertyDeedDTO)
    {
        ArgumentNullException.ThrowIfNull(propertyDeedDTO);

        var propertyDeed = new PropertyDeed
        {
            DeedNumber = propertyDeedDTO.DeedNumber,
            RegistryNumber = propertyDeedDTO.RegistryNumber,
            IssueDate = propertyDeedDTO.IssueDate,
            IssuedBy = propertyDeedDTO.IssuedBy,
            ImageId = propertyDeedDTO.ImageId,
            PropertyId = propertyDeedDTO.PropertyId
        };

        return await _repository.AddAsync(propertyDeed).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, PropertyDeedDTO propertyDeedDTO)
    {

        ArgumentNullException.ThrowIfNull(propertyDeedDTO);

        var propertyDeed = new PropertyDeed
        {
            Id = id,
            DeedNumber = propertyDeedDTO.DeedNumber,
            RegistryNumber = propertyDeedDTO.RegistryNumber,
            IssueDate = propertyDeedDTO.IssueDate,
            IssuedBy = propertyDeedDTO.IssuedBy,
            ImageId = propertyDeedDTO.ImageId,
            PropertyId = propertyDeedDTO.PropertyId
        };

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
