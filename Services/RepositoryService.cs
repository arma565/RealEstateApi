using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public class RepositoryService
{
    private readonly AppDbContext _context;
    public RepositoryService(AppDbContext context)
    {
        _context = context;
    }

    #region 
    public async Task<IEnumerable<Property>> GetPropertyList() =>
    await _context.
    Properties.
    AsNoTracking().
    Include(prop => prop.Persons).
    OrderByDescending(prop => prop.Id).ToListAsync();

    public async Task<Property?> GetProperty(int propertyID) =>
     await _context.
     Properties.
     AsNoTracking().
     SingleOrDefaultAsync(prop => prop.Id == propertyID);

    public async Task<bool> GetPropertyByPlateNumber(string plateNumber) =>
    await _context.
    Properties.
    AsNoTracking().
    AnyAsync(prop => prop.PlatesNumber == plateNumber);

    public async Task<Property?> AddProperty(Property newProperty)
    {
        await _context.Properties.AddAsync(newProperty);
        await _context.SaveChangesAsync();
        return newProperty;
    }

    public void UpdateProperty(Property updateProperty)
    {
        _context.Properties.Update(updateProperty);
        _context.SaveChanges();
    }

    public void DeleteProperty(Property deleteProperty)
    {
        _context.Properties.Remove(deleteProperty);
        _context.SaveChanges();
    }

    public void DeleteAllProperties()
    {
        _context.Properties.ExecuteDelete();
        _context.SaveChanges();
    }
    #endregion

    #region
    public async Task<Person?> GetPerson(int id) => await _context.Persons.AsNoTracking().SingleOrDefaultAsync(pers => pers.Id == id);

    public async Task<bool> GetPersonByPersonID(long personID) => await _context.Persons.AsNoTracking().AnyAsync(pers => pers.PersonID == personID);

    public async Task<Person> AddPerson(Person newPerson)
    {
        await _context.Persons.AddAsync(newPerson);
        await _context.SaveChangesAsync();
        return newPerson;
    }

    public void UpdatePerson(Person updatePerson)
    {
        _context.Persons.Update(updatePerson);
        _context.SaveChanges();
    }

    public void DeletePerson(Person deletePerson)
    {
        _context.Persons.Remove(deletePerson);
        _context.SaveChanges();
    }

    public void DeleteAllPersons()
    {
        _context.Persons.ExecuteDelete();
        _context.SaveChanges();
    }
    private IEnumerable<Person> GetPersonList() => [.. _context.Persons.AsNoTracking().OrderByDescending(pers => pers.Id)];
    #endregion


}