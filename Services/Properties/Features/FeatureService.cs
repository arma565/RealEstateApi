using RealEstate.DTOs.Properties.Features;
using RealEstate.Entities.Properties.Features;
using RealEstate.Repositories.Properties.Features;

namespace RealEstate.Services.Properties.Features;

interface IFeatureService
{
    Task<IEnumerable<PropertyFeature>> GetListAsync();
    Task<PropertyFeature> GetAsync(Guid id);
    Task<PropertyFeature> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class FeatureService(FeatureRepository<PropertyFeature> repository) : IFeatureService
{
    private readonly FeatureRepository<PropertyFeature> _repository = repository;

    public async Task<IEnumerable<PropertyFeature>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyFeature> GetAsync(Guid id)
    {
        var propertyFeature = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyFeature);

        return propertyFeature;
    }

    public async Task<PropertyFeature> AddAsync(CreateDTO createDTO)
    {
        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new PropertyFeature
        {
            Name = createDTO.Name,
            PropertyFeatureCategory = createDTO.Category,
            PropertyId = createDTO.PropertyId
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO)
    {

        ArgumentNullException.ThrowIfNull(updateDTO);

        var propertyFeature = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyFeature);

        propertyFeature.Name = string.IsNullOrEmpty(updateDTO.Name) ? propertyFeature.Name : updateDTO.Name;
        propertyFeature.PropertyFeatureCategory = updateDTO.PropertyFeatureCategory;
        propertyFeature.PropertyId = updateDTO.PropertyId != propertyFeature.PropertyId ? updateDTO.PropertyId : propertyFeature.PropertyId;

        await _repository.UpdateAsync(propertyFeature).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var feature = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(feature);

        await _repository.DeleteAsync(feature).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
        await _repository.DeleteAllAsync().ConfigureAwait(false);
}
