using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Models.Images;
using RealEstate.Models.Property;
using RealEstate.Services.Images;

namespace RealEstate.Services.Repositories.Properties;

interface IPropertyImageRepository
{
    Task<List<RealEstateImage>> GetListAsync();
    Task<RealEstateImage?> GetByIdAsync(Guid propertyImageId);
    Task AddAsync(RealEstateImage propertyImage);
    Task DeleteAsync(IEnumerable<RealEstateImage> propertyImages);
}
#pragma warning disable CA1515
public class PropertyImageRepository(AppDbContext context,
                                        ImageService imageService) : IPropertyImageRepository
{
    private readonly AppDbContext _context = context;

    private readonly ImageService _imageService = imageService;

    public async Task<List<RealEstateImage>> GetListAsync() =>
    await _context
       .Images
       .AsNoTracking()
       .OrderByDescending(assetImg => assetImg.Id)
       .ToListAsync()
       .ConfigureAwait(false);

    public async Task<RealEstateImage?> GetByIdAsync(Guid propertyImageId) =>
   await _context
    .Images.AsNoTracking()
    .SingleOrDefaultAsync(propertyImg => propertyImg.Id == propertyImageId)
    .ConfigureAwait(false);

    public async Task AddAsync(RealEstateImage propertyImage) {
        await _context.Images.AddAsync(propertyImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
    public async Task DeleteAsync(IEnumerable<RealEstateImage> propertyImages) =>
        await _imageService.DeleteImages(propertyImages).ConfigureAwait(false);
}
