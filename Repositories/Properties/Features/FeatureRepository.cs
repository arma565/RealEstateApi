using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Features;

namespace RealEstate.Repositories.Properties.Features;

interface IFeatureRepository
{
    Task<IEnumerable<PropertyFeature>> GetListAsync();
    Task<PropertyFeature?> GetAsync(Guid id);
    Task<PropertyFeature> AddAsync(PropertyFeature propertyFeature);
    Task UpdateAsync(PropertyFeature propertyFeature);
    Task DeleteAsync(PropertyFeature propertyFeature);
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

    public async Task<PropertyFeature> AddAsync(PropertyFeature propertyFeature)
    {
        await _context.PropertyFeatures.AddAsync(propertyFeature).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyFeature;
    }

    public async Task UpdateAsync(PropertyFeature propertyFeature)
    {
        _context.PropertyFeatures.Update(propertyFeature);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(PropertyFeature propertyFeature)
    {
        _context.PropertyFeatures.Remove(propertyFeature);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.PropertyFeatures.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
