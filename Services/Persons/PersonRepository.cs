using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Persons;

namespace RealEstate.Repositories.Persons;

interface IPersonRepository
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
public class PersonRepository(AppDbContext context) : IPersonRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Person>> GetListAsync() =>
        await _context
            .Persons.AsNoTracking()
            .OrderByDescending(per => per.Id)
            .ToListAsync().ConfigureAwait(false);

    public async Task<Person?> GetAsync(Guid id) =>
        await _context.Persons.AsNoTracking().SingleOrDefaultAsync(person => person.Id == id).ConfigureAwait(false);

    public async Task<Person?> GetByNationalIdAsync(long nationalId) =>
        await _context.Persons.AsNoTracking().SingleOrDefaultAsync(person => person.NationalId == nationalId).ConfigureAwait(false);

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

        await _context.Persons.AddAsync(person).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return person;
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

        _context.Persons.Update(person);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var person = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(person);

        _context.Persons.Remove(person);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Persons.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}



