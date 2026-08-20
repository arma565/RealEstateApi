using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Supports;

namespace RealEstate.Repositories.Supports;

#pragma warning disable CA1515
public class SupportRepository<TRealEstateSupport>(AppDbContext context) : BaseRepository<RealEstateSupport>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<RealEstateSupport>> GetListAsync() =>
       await _context
      .Supports
      .AsNoTracking()
      .Include(support => support.SupportImage)
      .ToListAsync().ConfigureAwait(false);

    public override async Task<RealEstateSupport?> GetAsync(Guid id) =>
      await _context
          .Supports.AsNoTracking()
          .Include(support => support.SupportImage)
          .SingleOrDefaultAsync(support => support.Id == id)
          .ConfigureAwait(false);

    public override async Task<RealEstateSupport> AddAsync(RealEstateSupport realEstateSupport)
    {
        await _context.Supports.AddAsync(realEstateSupport).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return realEstateSupport;
    }

    public override async Task UpdateAsync(RealEstateSupport realEstateSupport)
    {
        _context.Supports.Update(realEstateSupport);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(RealEstateSupport realEstateSupport)
    {
        _context.Supports.Remove(realEstateSupport);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.Supports.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}



