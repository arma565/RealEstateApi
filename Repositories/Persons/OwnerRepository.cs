using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Persons.Owners;

namespace RealEstate.Repositories.Owners;

interface IOwnerRepository
{
    Task<IEnumerable<Owner>> GetListAsync();
    Task<Owner?> GetAsync(Guid id);
    Task<Owner> AddAsync(Owner owner);
    Task UpdateAsync(Owner owner);
    Task DeleteAsync(Owner owner);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class OwnerRepository(AppDbContext context) : IOwnerRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Owner>> GetListAsync() =>
        await _context
            .Owners
            .Include(owner => owner.RealEstateProperties)
            .Include(owner => owner.Leases)
            .AsNoTracking()
            .OrderByDescending(per => per.Id)
            .ToListAsync().ConfigureAwait(false);

    public async Task<Owner?> GetAsync(Guid id) =>
         await _context
            .Owners
            .Include(owner => owner.RealEstateProperties)
            .Include(owner => owner.Leases)
            .AsNoTracking()
            .SingleOrDefaultAsync(owner => owner.Id == id)
            .ConfigureAwait(false);

    public async Task<Owner> AddAsync(Owner owner)
    {
        await _context.Owners.AddAsync(owner).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return owner;
    }

    public async Task UpdateAsync(Owner owner)
    {
        _context.Owners.Update(owner);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Owner owner)
    {
        _context.Owners.Remove(owner);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Owners.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}



