using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Properties.Documents;

namespace RealEstate.Services.Repositories.Properties.Documents;

interface IDeedRepository
{
    Task<IEnumerable<PropertyDeed>> GetListAsync();
    Task<PropertyDeed?> GetAsync(Guid id);
    Task<PropertyDeed> AddAsync(PropertyDeedDTO propertyDeedDTO);
    Task UpdateAsync(Guid id,PropertyDeedDTO propertyDeedDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
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

    public async Task<PropertyDeed?> GetAsync(Guid id) =>
    await _context
       .PropertyDeeds
       .AsNoTracking()
       .Include(deed => deed.Image)
       .Include(deed => deed.Property)
       .SingleOrDefaultAsync(deed => deed.Id == id)
       .ConfigureAwait(false);

    public async Task<PropertyDeed> AddAsync(PropertyDeedDTO propertyDeedDTO)
    {
        ArgumentNullException.ThrowIfNull(propertyDeedDTO);

        var propertyDeed = new PropertyDeed
        {
            DeedNumber = propertyDeedDTO.DeedNumber,
            RegistryNumber = propertyDeedDTO.RegistryNumber,
            IssueDate = propertyDeedDTO.IssueDate,
            IssuedBy = propertyDeedDTO.IssuedBy,
            ImageId = propertyDeedDTO.ImageId,
            PropertyId = propertyDeedDTO.PropertyId
        };

        await _context.PropertyDeeds.AddAsync(propertyDeed).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyDeed;
    }

    public async Task UpdateAsync(Guid id,PropertyDeedDTO propertyDeedDTO)
    {

        ArgumentNullException.ThrowIfNull(propertyDeedDTO);

        var propertyDeed = new PropertyDeed
        {
            Id = id,
            DeedNumber = propertyDeedDTO.DeedNumber,
            RegistryNumber = propertyDeedDTO.RegistryNumber,
            IssueDate = propertyDeedDTO.IssueDate,
            IssuedBy = propertyDeedDTO.IssuedBy,
            ImageId = propertyDeedDTO.ImageId,
            PropertyId = propertyDeedDTO.PropertyId
        };

        _context.PropertyDeeds.Update(propertyDeed);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var propertyDeed = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(propertyDeed);

        _context.PropertyDeeds.Remove(propertyDeed);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.PropertyDeeds.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}
