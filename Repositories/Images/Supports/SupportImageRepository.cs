using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Images.Supports;

namespace RealEstate.Repositories.Images.Supports;


interface ISupportImageRepository
{
    Task<IEnumerable<SupportImage>> GetListAsync();
    Task<SupportImage?> GetAsync(Guid id);
    Task<SupportImage> AddAsync(SupportImage supportImage);
    Task UpdateAsync(SupportImage supportImage);
    Task DeleteAsync(SupportImage supportImage);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class SupportImageRepository(AppDbContext context) : ISupportImageRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<SupportImage>> GetListAsync() =>
         await _context
            .SupportImages
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<SupportImage?> GetAsync(Guid id) =>
    await _context
    .SupportImages
    .AsNoTracking()
    .SingleOrDefaultAsync(image => image.Id == id)
    .ConfigureAwait(false);

    public async Task<SupportImage> AddAsync(SupportImage supportImage)
    {
        await _context.SupportImages.AddAsync(supportImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return supportImage;
    }

    public async Task UpdateAsync(SupportImage supportImage)
    {
        _context.SupportImages.Update(supportImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(SupportImage supportImage)
    {
        _context.SupportImages.Remove(supportImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.SupportImages.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
