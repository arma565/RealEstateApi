using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Images.Properties;
using RealEstate.Images;

namespace RealEstate.Repositories.Images.Properties;


interface IPropertyImageService
{
    Task<IEnumerable<PropertyImage>> GetListAsync();
    Task<PropertyImage?> GetAsync(Guid id);
    Task<DownloadPaths> GetPathAsync(Guid id);
    Task<PropertyImage> AddAsync(PropertyImageDTO propertyImageDTO);
    Task UpdateAsync(Guid id, PropertyImageDTO propertyImageDTO, IFormFile image);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PropertyImageService(AppDbContext context,
                                        ImageService imageService) : IPropertyImageService
{
    private readonly AppDbContext _context = context;

    private readonly ImageService _imageService = imageService;

    public async Task<IEnumerable<PropertyImage>> GetListAsync() =>
         await _context
            .PropertyImages
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<PropertyImage?> GetAsync(Guid id) =>
    await _context
    .PropertyImages
    .AsNoTracking()
    .SingleOrDefaultAsync(image => image.Id == id)
    .ConfigureAwait(false);

    public async Task<PropertyImage> AddAsync(PropertyImageDTO propertyImageDTO)
    {
        ArgumentNullException.ThrowIfNull(propertyImageDTO);

        var propertyImage = new PropertyImage
        {
            PropertyId = propertyImageDTO.PropertyId
        };
        await _context.PropertyImages.AddAsync(propertyImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return propertyImage;
    }

    public async Task UpdateAsync(Guid id, PropertyImageDTO propertyImageDTO, IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(propertyImageDTO);

        var propertyImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyImage);

        var savedImagePaths = await _imageService.SaveAsync(image).ConfigureAwait(false);

        propertyImage.Id = id;
        propertyImage.Order = propertyImage.Order++;
        propertyImage.IsCoverImage = propertyImage.Order == 1;
        propertyImage.ImageFilePath = savedImagePaths.OriginalPath;
        propertyImage.ThumbnailFilePath = savedImagePaths.ThumbnailPath;
        propertyImage.PropertyId = propertyImageDTO.PropertyId;

        _context.PropertyImages.Update(propertyImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {

        var propertyImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(propertyImage);

        await _imageService.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = propertyImage.ImageFilePath,
            ThumbnailPath = propertyImage.ThumbnailFilePath
        }).ConfigureAwait(false);

        _context.PropertyImages.Remove(propertyImage);
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

    public async Task<DownloadPaths> GetPathAsync(Guid id)
    {
        var propertyImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(propertyImage);

        return await _imageService.GetPathsAsync(new ImagePaths
        {
            OriginalPath = propertyImage.ImageFilePath,
            ThumbnailPath = propertyImage.ThumbnailFilePath
        }).ConfigureAwait(false);
    }
}
