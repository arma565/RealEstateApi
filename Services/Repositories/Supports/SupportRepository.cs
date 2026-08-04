using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Supports;
using RealEstate.Services.Repositories.Images;


namespace RealEstate.Services.Repositories.Supports;

interface ISupportRepository
{
    Task<IEnumerable<RealEstateSupport>> GetListAsync();
    Task<RealEstateSupport?> GetAsync(Guid id);
    Task AddAsync(RealEstateSupport support);
    Task UpdateAsync(RealEstateSupport support);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
    Task<bool> IsSupportExistAsync(Guid id);
}

#pragma warning disable CA1515
public class SupportRepository(AppDbContext context,
                                        ImageRepository imageRepository) : ISupportRepository
{
    private readonly AppDbContext _context = context;

    private readonly ImageRepository _imageRepository = imageRepository;

    public async Task<IEnumerable<RealEstateSupport>> GetListAsync() =>
       await _context
      .Supports
      .AsNoTracking()
      .Include(support => support.Image)
      .ToListAsync().ConfigureAwait(false);

    public async Task<RealEstateSupport?> GetAsync(Guid id) =>
      await _context
          .Supports.AsNoTracking()
          .Include(sups => sups.Image)
          .SingleOrDefaultAsync(support => support.Id == id)
          .ConfigureAwait(false);

    public async Task AddAsync(RealEstateSupport support)
    {
        await _context.Supports.AddAsync(support).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateAsync(RealEstateSupport support)
    {
        _context.Supports.Update(support);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var support = await _context.Supports.FindAsync(id).ConfigureAwait(false);

        if (support == null || support.Image == null)
            ArgumentNullException.ThrowIfNull(support!.Image);

        await _imageRepository.DeleteAsync(support.Image.Id).ConfigureAwait(false);

        _context.Supports.Remove(support);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var supports = await GetListAsync().ConfigureAwait(false);
        foreach (var support in supports)
        {
            if (support == null || support.Image == null)
                continue;
            await _imageRepository.DeleteAsync(support.Image.Id).ConfigureAwait(false);
        }
        _context.Supports.ExecuteDelete();
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<bool> IsSupportExistAsync(Guid id) =>
   (await GetListAsync().ConfigureAwait(false)).Any(support => support.Id == id);

}



