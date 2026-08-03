using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Images;
using RealEstate.Services.Models.Images;

namespace RealEstate.Services.Repositories.Images;

interface IImageRepository
{
    Task UploadAsync(IFormFile image, Guid imageId);
    Task<DownloadPaths> DownloadAsync(Guid imageId);
    Task<RealEstateImage?> GetByIdAsync(Guid imageId);
    Task<RealEstateImage> AddAsync(RealEstateImageDTO image);
    Task DeleteAsync(Guid imageId);
}

#pragma warning disable CA1515
public class ImageRepository(AppDbContext context,
                                        ImageService imageService) : IImageRepository
{
    private readonly AppDbContext _context = context;

    private readonly ImageService _imageService = imageService;


    public async Task UploadAsync(IFormFile image , Guid imageId)
    {
        var lastSavedImage = await _context.Images.LastOrDefaultAsync().ConfigureAwait(false) ?? null;

        var lastImageOrderNumber = (lastSavedImage != null) ? lastSavedImage.Order : 0;

        var imagePaths = await _imageService.SaveAsync(image, lastImageOrderNumber).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(imagePaths);

        var realEstateImage = await _context.Images.SingleOrDefaultAsync(img => img.Id == imageId).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(realEstateImage);

        var isExist = await _context.Images.AnyAsync(img => img.Id == realEstateImage.Id).ConfigureAwait(false);

        if (!isExist)
            throw new InvalidOperationException("realEstateImage is not found!");

        realEstateImage.ImageFilePath = imagePaths.OriginalPath;
        realEstateImage.ThumbnailFilePath = imagePaths.ThumbnailPath;

        _context.Images.Update(realEstateImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<DownloadPaths> DownloadAsync(Guid imageId)
    {
        var realEstateImage = await _context.Images.SingleOrDefaultAsync(img => img.Id == imageId).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(realEstateImage);

        var isExist = await _context.Images.AnyAsync(img => img.Id == realEstateImage.Id).ConfigureAwait(false);

        if (!isExist)
            throw new InvalidOperationException("realEstateImage is not found!");

        return await _imageService.GetPathsAsync(new ImagePaths
        {
            Order = realEstateImage.Order,
            OriginalPath = realEstateImage.ImageFilePath,
            ThumbnailPath = realEstateImage.ThumbnailFilePath
        }).ConfigureAwait(false);
    }

    public async Task<RealEstateImage?> GetByIdAsync(Guid imageId) =>
    await _context
    .Images.AsNoTracking()
    .SingleOrDefaultAsync(image => image.Id == imageId)
    .ConfigureAwait(false);

    public async Task<RealEstateImage> AddAsync(RealEstateImageDTO image) {

        ArgumentNullException.ThrowIfNull(image);

        var realEstateImage = new RealEstateImage
        {
            UserId = image.UserId,
            PropertyId = image.PropertyId,
            PropertyDeedId = image.PropertyDeedId,
            SupportId = image.SupportId
        };
        await _context.Images.AddAsync(realEstateImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return realEstateImage;
    }

    public async Task DeleteAsync(Guid imageId) {

        var realEstateImage = await _context.Images.SingleOrDefaultAsync(img => img.Id == imageId).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(realEstateImage);

        var isExist = await _context.Images.AnyAsync(img => img.Id == realEstateImage.Id).ConfigureAwait(false);

        if (!isExist)
            throw new InvalidOperationException("realEstateImage is not found!");

        await _imageService.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = realEstateImage.ImageFilePath,
            ThumbnailPath = realEstateImage.ThumbnailFilePath
        }).ConfigureAwait(false);

        _context.Images.Remove(realEstateImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);

    }
        

    
}
