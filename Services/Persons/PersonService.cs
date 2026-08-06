using RealEstate.Entities.Persons;
using RealEstate.Repositories.Persons;

namespace RealEstate.Services.Persons;

interface IPersonService
{
    Task<IEnumerable<Person>> GetListAsync();
    Task<Person?> GetAsync(Guid id);
    Task<Person?> GetByNationalIdAsync(long nationalId);
    Task<Person> AddAsync(PersonDTO personDTO);
    Task UpdateAsync(PersonDTO personDTO, Guid id);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PersonService(PersonRepository repository) : IPersonService
{
    private readonly PersonRepository _repository = repository;

    public async Task<IEnumerable<Person>> GetListAsync() =>
     await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<Person?> GetAsync(Guid id) =>
         await _repository.GetAsync(id).ConfigureAwait(false);

    public async Task<Person?> GetByNationalIdAsync(long nationalId) =>
        await _repository.GetByNationalIdAsync(nationalId).ConfigureAwait(false);

    public async Task<Person> AddAsync(PersonDTO personDTO)
    {
        ArgumentNullException.ThrowIfNull(personDTO);

        var person = new Person
        {
            FirstName = personDTO.FirstName,
            LastName = personDTO.LastName,
            FatherName = personDTO.FatherName,
            BirthCertificateNumber = personDTO.BirthCertificateNumber,
            BirthCertificateIssued = personDTO.BirthCertificateIssued,
            NationalId = personDTO.NationalId,
            Born = personDTO.Born,
            Phone = personDTO.Phone,
            Address = personDTO.Address,
            Role = personDTO.Role,
            PropertyId = personDTO.PropertyId,
            LeaseId = personDTO.LeaseId
        };

        return await _repository.AddAsync(person).ConfigureAwait(false);
    }

    public async Task UpdateAsync(PersonDTO personDTO, Guid id)
    {
        ArgumentNullException.ThrowIfNull(personDTO);

        var person = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(person);

        person.Id = id;
        person.FirstName = personDTO.FirstName;
        person.LastName = personDTO.LastName;
        person.FatherName = personDTO.FatherName;
        person.BirthCertificateNumber = personDTO.BirthCertificateNumber;
        person.BirthCertificateIssued = personDTO.BirthCertificateIssued;
        person.NationalId = personDTO.NationalId;
        person.Born = personDTO.Born;
        person.Phone = personDTO.Phone;
        person.Address = personDTO.Address;
        person.Role = personDTO.Role;
        person.PropertyId = personDTO.PropertyId;
        person.LeaseId = personDTO.LeaseId;

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



