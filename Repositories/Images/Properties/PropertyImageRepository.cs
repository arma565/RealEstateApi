using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Images.Properties;

namespace RealEstate.Repositories.Images.Properties;


interface IPropertyImageRepository
{
    Task<IEnumerable<PropertyImage>> GetListAsync();
    Task<PropertyImage?> GetAsync(Guid id);
    Task<PropertyImage> AddAsync(PropertyImage propertyImage);
    Task UpdateAsync(PropertyImage propertyImage);
    Task DeleteAsync(PropertyImage propertyImage);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PropertyImageRepository(AppDbContext context) : IPropertyImageRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PropertyImage>> GetListAsync() =>
         await _context
            .PropertyImages
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<PropertyImage?> GetAsync(Guid id) =>
         await _context
            .PropertyImages
            .AsNoTracking()
            .SingleOrDefaultAsync(image => image.Id == id)
            .ConfigureAwait(false);

    public async Task<PropertyImage> AddAsync(PropertyImage propertyImage)
    {
        await _context.PropertyImages.AddAsync(propertyImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyImage;
    }

    public async Task UpdateAsync(PropertyImage propertyImage)
    {
        _context.PropertyImages.Update(propertyImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(PropertyImage propertyImage)
    {
        _context.PropertyImages.Remove(propertyImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.PropertyImages.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
