using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Features;

namespace RealEstate.Repositories.Properties.Features;

interface IFeatureRepository
{
    Task<IEnumerable<PropertyFeature>> GetListAsync();
    Task<PropertyFeature?> GetAsync(Guid id);
    Task<PropertyFeature> AddAsync(PropertyFeatureDTO propertyFeatureDTO);
    Task UpdateAsync(Guid id, PropertyFeatureDTO propertyFeatureDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class FeatureRepository(AppDbContext context) : IFeatureRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PropertyFeature>> GetListAsync() =>
     await _context
        .PropertyFeatures
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<PropertyFeature?> GetAsync(Guid id) =>
    await _context
       .PropertyFeatures
       .AsNoTracking()
       .SingleOrDefaultAsync(feature => feature.Id == id)
       .ConfigureAwait(false);

    public async Task<PropertyFeature> AddAsync(PropertyFeatureDTO propertyFeatureDTO)
    {
        ArgumentNullException.ThrowIfNull(propertyFeatureDTO);

        var feature = new PropertyFeature
        {
            Name = propertyFeatureDTO.Name,
            Category = propertyFeatureDTO.Category,
            PropertyId = propertyFeatureDTO.PropertyId
        };

        await _context.PropertyFeatures.AddAsync(feature).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return feature;
    }

    public async Task UpdateAsync(Guid id, PropertyFeatureDTO propertyFeatureDTO)
    {

        ArgumentNullException.ThrowIfNull(propertyFeatureDTO);

        var feature = new PropertyFeature
        {
            Id = id,
            Name = propertyFeatureDTO.Name,
            Category = propertyFeatureDTO.Category,
            PropertyId = propertyFeatureDTO.PropertyId
        };

        _context.PropertyFeatures.Update(feature);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var feature = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(feature);

        _context.PropertyFeatures.Remove(feature);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.PropertyFeatures.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
