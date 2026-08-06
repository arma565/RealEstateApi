using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Supports;


namespace RealEstate.Repositories.Supports;

interface ISupportRepository
{
    Task<IEnumerable<RealEstateSupport>> GetListAsync();
    Task<RealEstateSupport?> GetAsync(Guid id);
    Task<RealEstateSupport> AddAsync(RealEstateSupport realEstateSupport);
    Task UpdateAsync( RealEstateSupport realEstateSupport);
    Task DeleteAsync(RealEstateSupport realEstateSupport);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class SupportRepository(AppDbContext context) : ISupportRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<RealEstateSupport>> GetListAsync() =>
       await _context
      .Supports
      .AsNoTracking()
      .Include(support => support.Image)
      .ToListAsync().ConfigureAwait(false);

    public async Task<RealEstateSupport?> GetAsync(Guid id) =>
      await _context
          .Supports.AsNoTracking()
          .Include(support => support.Image)
          .SingleOrDefaultAsync(support => support.Id == id)
          .ConfigureAwait(false);

    public async Task<RealEstateSupport> AddAsync(RealEstateSupport realEstateSupport)
    {
        await _context.Supports.AddAsync(realEstateSupport).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return realEstateSupport;
    }

    public async Task UpdateAsync(RealEstateSupport realEstateSupport)
    {
        _context.Supports.Update(realEstateSupport);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(RealEstateSupport realEstateSupport)
    {
        _context.Supports.Remove(realEstateSupport);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.Supports.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}



