using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Images;
using RealEstate.Services.Models.Properties;

namespace RealEstate.Services.Repositories.Properties;

interface IPropertyRepository
{
    Task<IEnumerable<RealEstateProperty>> GetListAsync();
    Task<RealEstateProperty?> GetByIdAsync(Guid id);
    Task AddAsync(RealEstateProperty property);
    Task UpdateAsync(RealEstateProperty property);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
    Task<RealEstateProperty?> FindByPlatesNumberAsync(int platesNumber);
    Task<bool> IsPropertyExistAsync(int plateNumber);
}

#pragma warning disable CA1515
public class PropertyRepository(AppDbContext context,
                                        ImageService imageService) : IPropertyRepository
{
    private readonly AppDbContext _context = context;

    private readonly ImageService _imageService = imageService;

    public async Task<IEnumerable<RealEstateProperty>> GetListAsync() =>
         await _context
            .Properties
            .AsNoTracking()
            .Include(property => property.Address)
            .Include(property => property.Location)
            .Include(property => property.Owner)
            .Include(property => property.Agent)
            .Include(property => property.PropertyDeed)
            .Include(property => property.Lease)
            .Include(property => property.Features)
            .Include(propertyImg => propertyImg.Images)
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<RealEstateProperty?> GetByIdAsync(Guid id) =>
        await _context
            .Properties
            .AsNoTracking()
             .Include(property => property.Address)
            .Include(property => property.Location)
            .Include(property => property.Owner)
            .Include(property => property.Agent)
            .Include(property => property.PropertyDeed)
            .Include(property => property.Lease)
            .Include(property => property.Features)
            .Include(propertyImg => propertyImg.Images)
            .SingleOrDefaultAsync(property => property.Id == id)
            .ConfigureAwait(false);

    public async Task AddAsync(RealEstateProperty property)
    {
        await _context.Properties.AddAsync(property).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(RealEstateProperty property)
    {
        _context.Properties.Update(property);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var property = await _context.Properties.FindAsync(id).ConfigureAwait(false);

        if (property == null)
            ArgumentNullException.ThrowIfNull(property);

        await _imageService.DeleteImages(property.Images).ConfigureAwait(false);

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var properties = await GetListAsync().ConfigureAwait(false);
        foreach (var property in properties)
        {
            await _imageService.DeleteImages(property.Images).ConfigureAwait(false);
        }
        await _context.Properties.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<RealEstateProperty?> FindByPlatesNumberAsync(int platesNumber) =>
        (await GetListAsync().ConfigureAwait(false)).SingleOrDefault(property => property.PlatesNumber == platesNumber);

    public async Task<bool> IsPropertyExistAsync(int plateNumber) =>
       (await GetListAsync().ConfigureAwait(false)).Any(property => property.PlatesNumber == plateNumber);
}



