using RealEstate.Entities.Images.Properties;
using RealEstate.Repositories.Images.Properties;

namespace RealEstate.Services.Images.Properties;


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
public class PropertyImageService(PropertyImageRepository
                                        repository, ImageProccess imageProccess) : IPropertyImageService
{
    private readonly PropertyImageRepository _repository = repository;

    private readonly ImageProccess _imageProccess = imageProccess;

    public async Task<IEnumerable<PropertyImage>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyImage?> GetAsync(Guid id) =>
        await _repository.GetAsync(id).ConfigureAwait(false);

    public async Task<PropertyImage> AddAsync(PropertyImageDTO propertyImageDTO)
    {
        ArgumentNullException.ThrowIfNull(propertyImageDTO);

        var propertyImage = new PropertyImage
        {
            PropertyId = propertyImageDTO.PropertyId
        };
        return await _repository.AddAsync(propertyImage).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, PropertyImageDTO propertyImageDTO, IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(propertyImageDTO);

        var propertyImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyImage);

        var savedImagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);

        propertyImage.Id = id;
        propertyImage.Order = propertyImage.Order++;
        propertyImage.IsCoverImage = propertyImage.Order == 1;
        propertyImage.ImageFilePath = savedImagePaths.OriginalPath;
        propertyImage.ThumbnailFilePath = savedImagePaths.ThumbnailPath;
        propertyImage.PropertyId = propertyImageDTO.PropertyId;

        await _repository.UpdateAsync(propertyImage).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var propertyImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(propertyImage);

        await _imageProccess.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = propertyImage.ImageFilePath,
            ThumbnailPath = propertyImage.ThumbnailFilePath
        }).ConfigureAwait(false);
        await _repository.DeleteAsync(propertyImage).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var propertyImages = await GetListAsync().ConfigureAwait(false);

        foreach (var propertyImage in propertyImages)
        {
            await _imageProccess.DeleteFilesAsync(new ImagePaths
            {
                OriginalPath = propertyImage.ImageFilePath,
                ThumbnailPath = propertyImage.ThumbnailFilePath
            }).ConfigureAwait(false);
        }
        await _repository.DeleteAllAsync().ConfigureAwait(false);
    }

    public async Task<DownloadPaths> GetPathAsync(Guid id)
    {
        var propertyImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(propertyImage);

        return await _imageProccess.GetPathsAsync(new ImagePaths
        {
            OriginalPath = propertyImage.ImageFilePath,
            ThumbnailPath = propertyImage.ThumbnailFilePath
        }).ConfigureAwait(false);
    }
}
