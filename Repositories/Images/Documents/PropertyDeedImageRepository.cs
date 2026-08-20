using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Images.Documents;

namespace RealEstate.Repositories.Images.Documents;

#pragma warning disable CA1515
public class PropertyDeedImageRepository<TPropertyDeedImage>(AppDbContext context) : BaseRepository<PropertyDeedImage>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<PropertyDeedImage>> GetListAsync() =>
         await _context
            .PropertyDeedImages
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

    public override async Task<PropertyDeedImage?> GetAsync(Guid id) =>
            await _context
            .PropertyDeedImages
            .AsNoTracking()
            .SingleOrDefaultAsync(image => image.Id == id)
            .ConfigureAwait(false);

    public override async Task<PropertyDeedImage> AddAsync(PropertyDeedImage propertyDeedImage)
    {
        await _context.PropertyDeedImages.AddAsync(propertyDeedImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyDeedImage;
    }

    public override async Task UpdateAsync(PropertyDeedImage propertyDeedImage)
    {
        _context.PropertyDeedImages.Update(propertyDeedImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(PropertyDeedImage propertyDeedImage)
    {
        _context.PropertyDeedImages.Remove(propertyDeedImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.PropertyDeedImages.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
