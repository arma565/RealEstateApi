using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Images.Supports;

namespace RealEstate.Repositories.Images.Supports;

#pragma warning disable CA1515
public class SupportImageRepository<TSupportImage>(AppDbContext context) : BaseRepository<SupportImage>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<SupportImage>> GetListAsync() =>
         await _context
            .SupportImages
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

    public override async Task<SupportImage?> GetAsync(Guid id) =>
         await _context
            .SupportImages
            .AsNoTracking()
            .SingleOrDefaultAsync(image => image.Id == id)
            .ConfigureAwait(false);

    public override async Task<SupportImage> AddAsync(SupportImage supportImage)
    {
        await _context.SupportImages.AddAsync(supportImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return supportImage;
    }

    public override async Task UpdateAsync(SupportImage supportImage)
    {
        _context.SupportImages.Update(supportImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(SupportImage supportImage)
    {
        _context.SupportImages.Remove(supportImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.SupportImages.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
