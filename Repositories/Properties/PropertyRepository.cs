using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties;

namespace RealEstate.Repositories.Properties;

interface IPropertyRepository
{
    Task<IEnumerable<RealEstateProperty>> GetListAsync();
    Task<RealEstateProperty?> GetAsync(Guid id);
    Task<RealEstateProperty> AddAsync(RealEstateProperty realEstateProperty);
    Task UpdateAsync(RealEstateProperty realEstateProperty);
    Task DeleteAsync(RealEstateProperty realEstateProperty);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PropertyRepository(AppDbContext context) : IPropertyRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<RealEstateProperty>> GetListAsync() =>
         await _context
            .Properties
            .AsNoTracking()
            .Include(property => property.Address)
            .Include(property => property.Location)
            .Include(property => property.Owner)
            .Include(property => property.PropertyDeed)
            .Include(property => property.Leases)
            .Include(property => property.PropertyFeatures)
            .Include(propertyImg => propertyImg.PropertyImages)
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<RealEstateProperty?> GetAsync(Guid id) =>
        await _context
            .Properties
            .AsNoTracking()
            .Include(property => property.Address)
            .Include(property => property.Location)
            .Include(property => property.Owner)
            .Include(property => property.PropertyDeed)
      .Include(property => property.Leases)
            .Include(property => property.PropertyFeatures)
            .Include(propertyImg => propertyImg.PropertyImages)
            .SingleOrDefaultAsync(property => property.Id == id)
            .ConfigureAwait(false);

    public async Task<RealEstateProperty> AddAsync(RealEstateProperty realEstateProperty)
    {
        await _context.Properties.AddAsync(realEstateProperty).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return realEstateProperty;
    }

    public async Task UpdateAsync(RealEstateProperty realEstateProperty)
    {
        _context.Properties.Update(realEstateProperty);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(RealEstateProperty realEstateProperty)
    {
        _context.Properties.Remove(realEstateProperty);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Properties.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
   
}



