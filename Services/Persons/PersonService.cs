using RealEstate.DTOs.Persons;
using RealEstate.Entities.Persons;
using RealEstate.Repositories.Persons;

namespace RealEstate.Services.Persons;

interface IPersonService
{
    Task<IEnumerable<Person>> GetListAsync();
    Task<Person> GetAsync(Guid id);
    Task<Person> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(UpdateDTO updateDTO, Guid id);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PersonService(PersonRepository repository) : IPersonService
{
    private readonly PersonRepository _repository = repository;

    public async Task<IEnumerable<Person>> GetListAsync() =>
     await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<Person> GetAsync(Guid id) {
        var person = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(person);
        return person;
    }

    public async Task<Person> AddAsync(CreateDTO createDTO)
    {
        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new Person
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
        ArgumentNullException.ThrowIfNull(updateDTO);

        var person = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(person);

        person.FirstName = string.IsNullOrEmpty(updateDTO.FirstName) ? person.FirstName : updateDTO.FirstName;
        person.LastName = string.IsNullOrEmpty(updateDTO.LastName) ? person.LastName : updateDTO.LastName;
        person.FatherName = string.IsNullOrEmpty(updateDTO.FatherName) ? person.FatherName : updateDTO.FatherName;
        person.BirthCertificateNumber = updateDTO.BirthCertificateNumber != person.BirthCertificateNumber ? updateDTO.BirthCertificateNumber : person.BirthCertificateNumber;
        person.BirthCertificateIssued = string.IsNullOrEmpty(updateDTO.BirthCertificateIssued) ? person.BirthCertificateIssued : updateDTO.BirthCertificateIssued;
        person.NationalId = updateDTO.NationalId != person.NationalId ? updateDTO.NationalId : person.NationalId;
        person.Born = string.IsNullOrEmpty(updateDTO.Born) ? person.Born : updateDTO.Born;
        person.Phone = string.IsNullOrEmpty(updateDTO.Phone) ? person.Phone : updateDTO.Phone;
        person.Address = string.IsNullOrEmpty(updateDTO.Address) ? person.Address : updateDTO.Address;

        await _repository.UpdateAsync(person).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var person = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(person);

        await _repository.DeleteAsync(person).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
        await _repository.DeleteAllAsync().ConfigureAwait(false);

}



