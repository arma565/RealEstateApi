using RealEstate.Entities.Properties.Addresses;
using RealEstate.Repositories.Properties.Addresses;

namespace RealEstate.Services.Properties.Addresses;

interface IAddressService
{
    Task<IEnumerable<PropertyAddress>> GetListAsync();
    Task<PropertyAddress?> GetAsync(Guid id);
    Task<PropertyAddress> AddAsync(PropertyAddressDTO propertyAddressDTO);
    Task UpdateAsync(Guid id, PropertyAddressDTO propertyAddressDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class AddressService(AddressRepository repository) : IAddressService
{
    private readonly AddressRepository _repository = repository;

    public async Task<IEnumerable<PropertyAddress>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyAddress?> GetAsync(Guid id) =>
        await _repository.GetAsync(id).ConfigureAwait(false);

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

      return  await _repository.AddAsync(address).ConfigureAwait(false);
       
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

        await _repository.UpdateAsync(address).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var address = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(address);

        await _repository.DeleteAsync(address).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
        await _repository.DeleteAllAsync().ConfigureAwait(false);

}
