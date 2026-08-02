using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Properties.Addresses;

namespace RealEstate.Services.Repositories.Properties.Addresses;

interface IAddressRepository
{
    Task<IEnumerable<PropertyAddress>> GetListAsync();
    Task<PropertyAddress?> GetByIdAsync(Guid id);
    Task AddAsync(PropertyAddress address);
    Task UpdateAsync(PropertyAddress address);
    Task DeleteAsync(Guid id);
}

#pragma warning disable CA1515
public class AddressRepository(AppDbContext context) : IAddressRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PropertyAddress>> GetListAsync() =>
     await _context
        .Addresses
        .AsNoTracking()
        .Include(address => address.Property)
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<PropertyAddress?> GetByIdAsync(Guid id) =>
    await _context
       .Addresses
       .AsNoTracking()
       .Include(address => address.Property)
       .SingleOrDefaultAsync(address => address.Id == id)
       .ConfigureAwait(false);

    public async Task AddAsync(PropertyAddress address)
    {
        await _context.Addresses.AddAsync(address).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(PropertyAddress address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var address = await _context.Addresses.FindAsync(id).ConfigureAwait(false);

        if (address == null)
            ArgumentNullException.ThrowIfNull(address);

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
