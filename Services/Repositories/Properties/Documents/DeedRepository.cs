using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Properties.Documents;

namespace RealEstate.Services.Repositories.Properties.Documents;

interface IDeedRepository
{
    Task<IEnumerable<PropertyDeed>> GetListAsync();
    Task<PropertyDeed?> GetByIdAsync(Guid id);
    Task AddAsync(PropertyDeed deed);
    Task UpdateAsync(PropertyDeed deed);
    Task DeleteAsync(Guid id);
}

#pragma warning disable CA1515
public class DeedRepository(AppDbContext context) : IDeedRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<PropertyDeed>> GetListAsync() =>
     await _context
        .PropertyDeeds
        .AsNoTracking()
        .Include(deed => deed.Image)
        .Include(deed => deed.Property)
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<PropertyDeed?> GetByIdAsync(Guid id) =>
    await _context
       .PropertyDeeds
       .AsNoTracking()
       .Include(deed => deed.Image)
       .Include(deed => deed.Property)
       .SingleOrDefaultAsync(deed => deed.Id == id)
       .ConfigureAwait(false);

    public async Task AddAsync(PropertyDeed deed)
    {
        await _context.PropertyDeeds.AddAsync(deed).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(PropertyDeed deed)
    {
        _context.PropertyDeeds.Update(deed);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var deed = await _context.PropertyDeeds.FindAsync(id).ConfigureAwait(false);

        if (deed == null)
            ArgumentNullException.ThrowIfNull(deed);

        _context.PropertyDeeds.Remove(deed);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
