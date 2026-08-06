using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties.Addresses;

namespace RealEstate.Repositories.Properties.Addresses;

interface IAddressRepository
{
    Task<IEnumerable<PropertyAddress>> GetListAsync();
    Task<PropertyAddress?> GetAsync(Guid id);
    Task<PropertyAddress> AddAsync(PropertyAddressDTO propertyAddressDTO);
    Task UpdateAsync(Guid id, PropertyAddressDTO propertyAddressDTO);
    Task DeleteAsync(Guid id);
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

    public async Task<PropertyAddress> AddAsync(PropertyAddressDTO propertyAddressDTO)
    {

        ArgumentNullException.ThrowIfNull(propertyAddressDTO);

        var address = new PropertyAddress
        {
            Country = propertyAddressDTO.Country,
            Province = propertyAddressDTO.Province,
            City = propertyAddressDTO.City,
            District = propertyAddressDTO.District,
            Street = propertyAddressDTO.Street,
            PlatesNumber = propertyAddressDTO.PlatesNumber,
            PostalCode = propertyAddressDTO.PostalCode,
            PropertyId = propertyAddressDTO.PropertyId
        };

        await _context.Addresses.AddAsync(address).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return address;
    }

    public async Task UpdateAsync(Guid id, PropertyAddressDTO propertyAddressDTO)
    {
        ArgumentNullException.ThrowIfNull(propertyAddressDTO);

        var address = new PropertyAddress
        {
            Id = id,
            Country = propertyAddressDTO.Country,
            Province = propertyAddressDTO.Province,
            City = propertyAddressDTO.City,
            District = propertyAddressDTO.District,
            Street = propertyAddressDTO.Street,
            PlatesNumber = propertyAddressDTO.PlatesNumber,
            PostalCode = propertyAddressDTO.PostalCode,
            PropertyId = propertyAddressDTO.PropertyId
        };

        _context.Addresses.Update(address);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var address = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(address);

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Addresses.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
