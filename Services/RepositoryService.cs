using Microsoft.EntityFrameworkCore;

public class RepositoryService
{
     private readonly AppDbContext _context;
    public RepositoryService(AppDbContext context)
    {
        _context = context;
    }

      #region 
    public IEnumerable<Property> GetPropertyList() => [.. _context.Properties.AsNoTracking().Include(prop =>prop.Persons).OrderByDescending(prop => prop.Id)];
    public Property? GetProperty(int propertyID) => _context.Properties.AsNoTracking().SingleOrDefault(prop => prop.Id == propertyID);
    public bool GetPropertyByPlateNumber(string plateNumber) => _context.Properties.AsNoTracking().Any(prop => prop.PlatesNumber == plateNumber);
    public Property? AddProperty(Property newProperty)
    {
        _context.Properties.Add(newProperty);
        _context.SaveChanges();
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
        _context.Properties.RemoveRange(GetPropertyList());
        _context.SaveChanges();
    }
    #endregion

    #region
    public Person? GetPerson(int id) => _context.Persons.AsNoTracking().SingleOrDefault(pers => pers.Id == id);

    public bool GetPersonByPersonID(long personID) => _context.Persons.AsNoTracking().Any(pers => pers.PersonID == personID);

    public Person AddPerson(Person newPerson){
        _context.Persons.Add(newPerson);
        _context.SaveChanges();
        return newPerson;
    }

    public void UpdatePerson(Person updatePerson){
        _context.Persons.Update(updatePerson);
        _context.SaveChanges();
    }

    public void DeletePerson(Person deletePerson){
        _context.Persons.Remove(deletePerson);
        _context.SaveChanges();
    }

    public void DeleteAllPersons(){
        _context.Persons.RemoveRange(GetPersonList());
        _context.SaveChanges();
    }
    private IEnumerable<Person> GetPersonList() => [.. _context.Persons.AsNoTracking().OrderByDescending(pers => pers.Id)];
    #endregion

  
}