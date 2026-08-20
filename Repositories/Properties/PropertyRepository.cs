using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties;

namespace RealEstate.Repositories.Properties;

#pragma warning disable CA1515
public class PropertyRepository<TRealEstateProperty>(AppDbContext context) : BaseRepository<RealEstateProperty>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<RealEstateProperty>> GetListAsync() =>
         await _context
            .Properties
            .AsNoTracking()
            .Include(property => property.Location)
            .Include(property => property.Address)
            .Include(property => property.PropertyDeed)
            .Include(property => property.Leases)
            .Include(property => property.PropertyFeatures)
            .Include(propertyImg => propertyImg.PropertyImages)
            .ToListAsync()
            .ConfigureAwait(false);

    public override async Task<RealEstateProperty?> GetAsync(Guid id) =>
        await _context
            .Properties
            .AsNoTracking()
            .Include(property => property.Location)
            .Include(property => property.Address)
            .Include(property => property.PropertyDeed)
            .Include(property => property.Leases)
            .Include(property => property.PropertyFeatures)
            .Include(propertyImg => propertyImg.PropertyImages)
            .SingleOrDefaultAsync(property => property.Id == id)
            .ConfigureAwait(false);

    public override async Task<RealEstateProperty> AddAsync(RealEstateProperty realEstateProperty)
    {
        await _context.Properties.AddAsync(realEstateProperty).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return realEstateProperty;
    }

    public override async Task UpdateAsync(RealEstateProperty realEstateProperty)
    {
        _context.Properties.Update(realEstateProperty);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(RealEstateProperty realEstateProperty)
    {
        _context.Properties.Remove(realEstateProperty);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.Properties.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}



