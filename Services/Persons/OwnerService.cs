using RealEstate.DTOs.Persons;
using RealEstate.Entities.Persons.Owners;
using RealEstate.Repositories.Owners;

namespace RealEstate.Services.Persons;

interface IOwnerService
{
    Task<IEnumerable<Owner>> GetListAsync();
    Task<Owner> GetAsync(Guid id);
    Task<Owner> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(UpdateDTO updateDTO, Guid id);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class OwnerService(OwnerRepository repository) : IOwnerService
{
    private readonly OwnerRepository _repository = repository;

    public async Task<IEnumerable<Owner>> GetListAsync() =>
     await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<Owner> GetAsync(Guid id) {
        var owner = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(owner);
        return owner;
    }

    public async Task<Owner> AddAsync(CreateDTO createDTO)
    {
        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new Owner
        {
            FirstName = createDTO.FirstName,
            LastName = createDTO.LastName,
            FatherName = createDTO.FatherName,
            BirthCertificateNumber = createDTO.BirthCertificateNumber,
            BirthCertificateIssued = createDTO.BirthCertificateIssued,
            NationalId = createDTO.NationalId,
            Born = createDTO.Born,
            Phone = createDTO.Phone,
            Address = createDTO.Address
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(UpdateDTO updateDTO, Guid id)
    {

        var owner = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(owner);

        ArgumentNullException.ThrowIfNull(updateDTO);

        owner.FirstName = string.IsNullOrEmpty(updateDTO.FirstName) ? owner.FirstName : updateDTO.FirstName;
        owner.LastName = string.IsNullOrEmpty(updateDTO.LastName) ? owner.LastName : updateDTO.LastName;
        owner.FatherName = string.IsNullOrEmpty(updateDTO.FatherName) ? owner.FatherName : updateDTO.FatherName;
        owner.BirthCertificateNumber = updateDTO.BirthCertificateNumber != owner.BirthCertificateNumber ? updateDTO.BirthCertificateNumber : owner.BirthCertificateNumber;
        owner.BirthCertificateIssued = string.IsNullOrEmpty(updateDTO.BirthCertificateIssued) ? owner.BirthCertificateIssued : updateDTO.BirthCertificateIssued;
        owner.NationalId = updateDTO.NationalId != owner.NationalId ? updateDTO.NationalId : owner.NationalId;
        owner.Born = string.IsNullOrEmpty(updateDTO.Born) ? owner.Born : updateDTO.Born;
        owner.Phone = string.IsNullOrEmpty(updateDTO.Phone) ? owner.Phone : updateDTO.Phone;
        owner.Address = string.IsNullOrEmpty(updateDTO.Address) ? owner.Address : updateDTO.Address;

        await _repository.UpdateAsync(owner).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var owner = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(owner);

        await _repository.DeleteAsync(owner).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
        await _repository.DeleteAllAsync().ConfigureAwait(false);

}



