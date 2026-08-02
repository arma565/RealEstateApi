using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Persons;

namespace RealEstate.Services.Repositories.Persons;

  interface IPersonRepository
{
    Task<IEnumerable<Person>> GetListAsync();
    Task<Person?> GetByIdAsync(Guid id);
    Task<bool> GetByPersonIDAsync(long nationalId);
    Task AddAsync(Person person);
    Task UpdateAsync(Person person);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
    Task<bool> IsPersonExistAsync(Guid id);
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

    public async Task<Person?> GetByIdAsync(Guid id) =>
        await _context.Persons.AsNoTracking().SingleOrDefaultAsync(pers => pers.Id == id).ConfigureAwait(false);

    public async Task<bool> GetByPersonIDAsync(long nationalId) =>
        await _context.Persons.AsNoTracking().AnyAsync(person => person.NationalId == nationalId).ConfigureAwait(false);

    public async Task AddAsync(Person person)
    {
        await _context.Persons.AddAsync(person).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(Person person)
    {
        _context.Persons.Update(person);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var person = await _context.Persons.FindAsync(id).ConfigureAwait(false);

        if (person == null)
            ArgumentNullException.ThrowIfNull(person);

        _context.Persons.Remove(person);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        _context.Persons.ExecuteDelete();
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<bool> IsPersonExistAsync(Guid id) =>
      await _context.Persons.AsNoTracking().AnyAsync(person => person.Id == id).ConfigureAwait(false);

}



