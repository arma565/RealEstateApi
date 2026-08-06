using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Images;
using RealEstate.Images;

namespace RealEstate.Repositories.Images;

interface IImageRepository
{
    Task<IEnumerable<RealEstateImage>> GetListAsync();
    Task<RealEstateImage?> GetAsync(Guid id);
    Task<RealEstateImage> AddAsync(RealEstateImage realEstateImage);
    Task UpdateAsync(RealEstateImage realEstateImage);
    Task DeleteAsync(RealEstateImage realEstateImage);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class ImageRepository(AppDbContext context) : IImageRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<RealEstateImage>> GetListAsync() =>
     await _context
        .Images
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);

    public async Task<RealEstateImage?> GetAsync(Guid id) =>
      await _context
      .Images.AsNoTracking()
      .SingleOrDefaultAsync(image => image.Id == id)
      .ConfigureAwait(false);

    public async Task<RealEstateImage> AddAsync(RealEstateImage realEstateImage) {
        await _context.Images.AddAsync(realEstateImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return realEstateImage;
    }

    public async Task UpdateAsync(RealEstateImage realEstateImage)
    {
        _context.Images.Update(realEstateImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(RealEstateImage realEstateImage) {
        _context.Images.Remove(realEstateImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.PropertyImages.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
