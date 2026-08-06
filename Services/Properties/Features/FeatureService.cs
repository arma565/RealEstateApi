using RealEstate.Entities.Properties.Features;
using RealEstate.Repositories.Properties.Features;

namespace RealEstate.Services.Properties.Features;

interface IFeatureService
{
    Task<IEnumerable<PropertyFeature>> GetListAsync();
    Task<PropertyFeature?> GetAsync(Guid id);
    Task<PropertyFeature> AddAsync(PropertyFeatureDTO propertyFeatureDTO);
    Task UpdateAsync(Guid id, PropertyFeatureDTO propertyFeatureDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class FeatureService(FeatureRepository repository) : IFeatureService
{
    private readonly FeatureRepository _repository = repository;

    public async Task<IEnumerable<PropertyFeature>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyFeature?> GetAsync(Guid id) =>
        await _repository.GetAsync(id).ConfigureAwait(false);   

    public async Task<PropertyFeature> AddAsync(PropertyFeatureDTO propertyFeatureDTO)
    {
        ArgumentNullException.ThrowIfNull(propertyFeatureDTO);

        var feature = new PropertyFeature
        {
            Name = propertyFeatureDTO.Name,
            PropertyFeatureCategory = propertyFeatureDTO.Category,
            PropertyId = propertyFeatureDTO.PropertyId
        };

        return await _repository.AddAsync(feature).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, PropertyFeatureDTO propertyFeatureDTO)
    {

        ArgumentNullException.ThrowIfNull(propertyFeatureDTO);

        var feature = new PropertyFeature
        {
            Id = id,
            Name = propertyFeatureDTO.Name,
            PropertyFeatureCategory = propertyFeatureDTO.Category,
            PropertyId = propertyFeatureDTO.PropertyId
        };

        await _repository.UpdateAsync(feature).ConfigureAwait(false);
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
