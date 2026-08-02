using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Images;
using RealEstate.Services.Models.Images;

namespace RealEstate.Services.Repositories.Images;

interface IImageRepository
{
    Task<RealEstateImage?> GetByIdAsync(Guid imageId);
    Task AddAsync(RealEstateImage image);
    Task DeleteAsync(IEnumerable<RealEstateImage> image);
}

#pragma warning disable CA1515
public class ImageRepository(AppDbContext context,
                                        ImageService imageService) : IImageRepository
{
    private readonly AppDbContext _context = context;

    private readonly ImageService _imageService = imageService;

    public async Task<RealEstateImage?> GetByIdAsync(Guid imageId) =>
    await _context
    .Images.AsNoTracking()
    .SingleOrDefaultAsync(image => image.Id == imageId)
    .ConfigureAwait(false);

    public async Task AddAsync(RealEstateImage image) {
        await _context.Images.AddAsync(image).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
    public async Task DeleteAsync(IEnumerable<RealEstateImage> image) =>
        await _imageService.DeleteImages(image).ConfigureAwait(false);
}
