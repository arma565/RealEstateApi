using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Addresses;

namespace RealEstate.Repositories.Properties.Addresses;

#pragma warning disable CA1515
public class AddressRepository<TPropertyAddress>(AppDbContext context) : BaseRepository<PropertyAddress>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<PropertyAddress>> GetListAsync() =>
     await _context
        .Addresses
        .Include(address => address.Property)
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);

    public override async Task<PropertyAddress?> GetAsync(Guid id) =>
    await _context
       .Addresses
       .Include(address => address.Property)
       .AsNoTracking()
       .SingleOrDefaultAsync(address => address.Id == id)
       .ConfigureAwait(false);

    public override async Task<PropertyAddress> AddAsync(PropertyAddress propertyAddress)
    {
        await _context.Addresses.AddAsync(propertyAddress).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyAddress;
    }

    public override async Task UpdateAsync(PropertyAddress propertyAddress)
    {
        _context.Addresses.Update(propertyAddress);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(PropertyAddress propertyAddress)
    {
        _context.Addresses.Remove(propertyAddress);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.Addresses.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
