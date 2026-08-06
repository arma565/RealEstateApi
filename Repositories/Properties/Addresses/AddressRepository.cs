using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Addresses;

namespace RealEstate.Repositories.Properties.Addresses;

interface IAddressRepository
{
    Task<IEnumerable<PropertyAddress>> GetListAsync();
    Task<PropertyAddress?> GetAsync(Guid id);
    Task<PropertyAddress> AddAsync(PropertyAddress propertyAddress);
    Task UpdateAsync(PropertyAddress propertyAddress);
    Task DeleteAsync(PropertyAddress propertyAddress);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class AddressRepository(AppDbContext context) : IAddressRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PropertyAddress>> GetListAsync() =>
     await _context
        .Addresses
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<PropertyAddress?> GetAsync(Guid id) =>
    await _context
       .Addresses
       .AsNoTracking()
       .SingleOrDefaultAsync(address => address.Id == id)
       .ConfigureAwait(false);

    public async Task<PropertyAddress> AddAsync(PropertyAddress propertyAddress)
    {
        await _context.Addresses.AddAsync(propertyAddress).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyAddress;
    }

    public async Task UpdateAsync(PropertyAddress propertyAddress)
    {
        _context.Addresses.Update(propertyAddress);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(PropertyAddress propertyAddress)
    {
        _context.Addresses.Remove(propertyAddress);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Addresses.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
