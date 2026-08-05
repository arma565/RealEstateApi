using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Images;
using RealEstate.Services.Models.Images;

namespace RealEstate.Services.Repositories.Images;

interface IImageRepository
{
    Task<IEnumerable<RealEstateImage>> GetListAsync();
    Task<DownloadPaths> GetPathAsync(Guid id);
    Task<RealEstateImage?> GetAsync(Guid id);
    Task<RealEstateImage> AddAsync(RealEstateImageDTO realEstateImageDTO);
    Task UpdateAsync(Guid id , RealEstateImageDTO realEstateImageDTO , IFormFile image);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class ImageRepository(AppDbContext context,
                                        ImageService imageService) : IImageRepository
{
    private readonly AppDbContext _context = context;

    private readonly ImageService _imageService = imageService;

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

    public async Task<DownloadPaths> GetPathAsync(Guid id)
    {
        var realEstateImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(realEstateImage);

        return await _imageService.GetPathsAsync(new ImagePaths
        {
            OriginalPath = realEstateImage.ImageFilePath,
            ThumbnailPath = realEstateImage.ThumbnailFilePath
        }).ConfigureAwait(false);
    }

    public async Task<RealEstateImage> AddAsync(RealEstateImageDTO realEstateImageDTO) {

        ArgumentNullException.ThrowIfNull(realEstateImageDTO);

        var realEstateImage = new RealEstateImage
        {
            UserId = realEstateImageDTO.UserId,
            PropertyDeedId = realEstateImageDTO.PropertyDeedId,
            SupportId = realEstateImageDTO.SupportId
        };
        await _context.Images.AddAsync(realEstateImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return realEstateImage;
    }

    public async Task UpdateAsync(Guid id , RealEstateImageDTO realEstateImageDTO, IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(realEstateImageDTO);

        var realEstateImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(realEstateImage);

        var imagePaths = await _imageService.SaveAsync(image).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(imagePaths);

        realEstateImage.Id = id;
        realEstateImage.ImageFilePath = imagePaths.OriginalPath;
        realEstateImage.ThumbnailFilePath = imagePaths.ThumbnailPath;
        realEstateImage.UserId = realEstateImageDTO.UserId;
        realEstateImage.PropertyDeedId = realEstateImageDTO.PropertyDeedId;
        realEstateImage.SupportId = realEstateImageDTO.SupportId;

        _context.Images.Update(realEstateImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id) {

        var realEstateImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(realEstateImage);

        await _imageService.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = realEstateImage.ImageFilePath,
            ThumbnailPath = realEstateImage.ThumbnailFilePath
        }).ConfigureAwait(false);

        _context.Images.Remove(realEstateImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var propertyImages = await GetListAsync().ConfigureAwait(false);

        foreach (var propertyImage in propertyImages)
        {
            await _imageService.DeleteFilesAsync(new ImagePaths
            {
                OriginalPath = propertyImage.ImageFilePath,
                ThumbnailPath = propertyImage.ThumbnailFilePath
            }).ConfigureAwait(false);
        }
        await _context.PropertyImages.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
