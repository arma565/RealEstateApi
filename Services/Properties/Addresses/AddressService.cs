using RealEstate.DTOs.Properties.Addresses;
using RealEstate.Entities.Properties.Addresses;
using RealEstate.Repositories.Properties.Addresses;

namespace RealEstate.Services.Properties.Addresses;

interface IAddressService
{
    Task<IEnumerable<PropertyAddress>> GetListAsync();
    Task<PropertyAddress> GetAsync(Guid id);
    Task<PropertyAddress> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class AddressService(AddressRepository repository) : IAddressService
{
    private readonly AddressRepository _repository = repository;

    public async Task<IEnumerable<PropertyAddress>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyAddress> GetAsync(Guid id)
    {
        var address = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(address);
        return address;
    }

    public async Task<PropertyAddress> AddAsync(CreateDTO createDTO)
    {

        ArgumentNullException.ThrowIfNull(createDTO);


        return await _repository.AddAsync(new PropertyAddress
        {
            Country = createDTO.Country,
            Province = createDTO.Province,
            City = createDTO.City,
            District = createDTO.District ?? "",
            Street = createDTO.Street,
            PlatesNumber = createDTO.PlatesNumber,
            PostalCode = createDTO.PostalCode ?? "",
            PropertyId = createDTO.PropertyId
        }).ConfigureAwait(false);

    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO)
    {
        ArgumentNullException.ThrowIfNull(updateDTO);

        var existAddress = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(existAddress);

        existAddress.Country = string.IsNullOrEmpty(updateDTO.Country) ? existAddress.Country : updateDTO.Country;
        existAddress.Province = string.IsNullOrEmpty(updateDTO.Province) ? existAddress.Province : updateDTO.Province;
        existAddress.City = string.IsNullOrEmpty(updateDTO.City) ? existAddress.City : updateDTO.City;
        existAddress.District = string.IsNullOrEmpty(updateDTO.District) ? existAddress.District : updateDTO.District;
        existAddress.Street = string.IsNullOrEmpty(updateDTO.Street) ? existAddress.Street : updateDTO.Street;
        existAddress.PlatesNumber = updateDTO.PlatesNumber.Equals(0) ? existAddress.PlatesNumber : updateDTO.PlatesNumber;
        existAddress.PostalCode = string.IsNullOrEmpty(updateDTO.PostalCode) ? existAddress.PostalCode : updateDTO.PostalCode;
        existAddress.PropertyId = updateDTO.PropertyId != updateDTO.PropertyId ? updateDTO.PropertyId : existAddress.PropertyId;

        await _repository.UpdateAsync(existAddress).ConfigureAwait(false);
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
