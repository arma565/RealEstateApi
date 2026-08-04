using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Properties.Features;

namespace RealEstate.Services.Repositories.Properties.Features;

interface IFeatureRepository
{
    Task<IEnumerable<PropertyFeature>> GetListAsync();
    Task<PropertyFeature?> GetAsync(Guid id);
    Task AddAsync(PropertyFeature feature);
    Task UpdateAsync(PropertyFeature feature);
    Task DeleteAsync(Guid id);
    Task<bool> IsFeatureExistAsync(Guid id);
}

#pragma warning disable CA1515
public class FeatureRepository(AppDbContext context) : IFeatureRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PropertyFeature>> GetListAsync() =>
     await _context
        .PropertyFeatures
        .AsNoTracking()
        .Include(feature => feature.Property)
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<PropertyFeature?> GetAsync(Guid id) =>
    await _context
       .PropertyFeatures
       .AsNoTracking()
       .Include(feature => feature.Property)
       .SingleOrDefaultAsync(feature => feature.Id == id)
       .ConfigureAwait(false);

    public async Task AddAsync(PropertyFeature feature)
    {
        await _context.PropertyFeatures.AddAsync(feature).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(PropertyFeature feature)
    {
        _context.PropertyFeatures.Update(feature);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var feature = await _context.PropertyFeatures.FindAsync(id).ConfigureAwait(false);

        if (feature == null)
            ArgumentNullException.ThrowIfNull(feature);

        _context.PropertyFeatures.Remove(feature);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<bool> IsFeatureExistAsync(Guid id) =>
        await _context.PropertyFeatures.AsNoTracking().AnyAsync(feature => feature.Id == id).ConfigureAwait(false);

}
