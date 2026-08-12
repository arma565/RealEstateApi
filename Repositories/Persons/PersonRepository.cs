using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Persons;

namespace RealEstate.Repositories.Persons;

interface IPersonRepository
{
    Task<IEnumerable<Person>> GetListAsync();
    Task<Person?> GetAsync(Guid id);
    Task<Person> AddAsync(Person person);
    Task UpdateAsync(Person person);
    Task DeleteAsync(Person person);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PersonRepository(AppDbContext context) : IPersonRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Person>> GetListAsync() =>
        await _context
            .Persons
            .Include(person => person.RealEstateProperties)
            .Include(person => person.Leases)
            .AsNoTracking()
            .OrderByDescending(per => per.Id)
            .ToListAsync().ConfigureAwait(false);

    public async Task<Person?> GetAsync(Guid id) =>
         await _context
            .Persons
            .Include(person => person.RealEstateProperties)
            .Include(person => person.Leases)
            .AsNoTracking()
            .SingleOrDefaultAsync(person => person.Id == id)
            .ConfigureAwait(false);

    public async Task<Person> AddAsync(Person person)
    {
        await _context.Persons.AddAsync(person).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return person;
    }

    public async Task UpdateAsync(Person person)
    {
        _context.Persons.Update(person);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Person person)
    {
        _context.Persons.Remove(person);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Persons.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}



