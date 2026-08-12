using RealEstate.DTOs.Images.Properties;
using RealEstate.Entities.Images.Properties;
using RealEstate.Repositories.Images.Properties;
using static System.Net.Mime.MediaTypeNames;

namespace RealEstate.Services.Images.Properties;


interface IPropertyImageService
{
    Task<IEnumerable<PropertyImage>> GetListAsync();
    Task<PropertyImage> GetAsync(Guid id);
    Task<DownloadPaths> GetPathAsync(Guid id);
    Task<PropertyImage> AddAsync(CreateDTO createDTO, IFormFile image);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO, IFormFile image);
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

    public async Task<PropertyImage> GetAsync(Guid id)
    {

        var propertyImage = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyImage);

        return propertyImage;
    }

    public async Task<PropertyImage> AddAsync(CreateDTO createDTO, IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(image);
        var savedImagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(createDTO);

        var allPropertyImages = await GetListAsync().ConfigureAwait(false);

        if (!allPropertyImages.Any())
        {
            createDTO.OrderId = 1;
            createDTO.IsCoverImage = true;
        }
        else
        {
            var lastPropertyImageOrderNumber = allPropertyImages.Last().OrderId;
            createDTO.OrderId = lastPropertyImageOrderNumber + 1;
            createDTO.IsCoverImage = false;
        }

        return await _repository.AddAsync(new PropertyImage
        {
            OrderId = createDTO.OrderId,
            IsCoverImage = createDTO.IsCoverImage,
            ImageFilePath = savedImagePaths.OriginalPath,
            ThumbnailFilePath = savedImagePaths.ThumbnailPath,
            PropertyId = createDTO.PropertyId
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO, IFormFile image)
    {
        var existPropertyImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(existPropertyImage);

        await _imageProccess.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = existPropertyImage.ImageFilePath,
            ThumbnailPath = existPropertyImage.ThumbnailFilePath
        }).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(updateDTO);

        ArgumentNullException.ThrowIfNull(image);
        var savedImagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);

        existPropertyImage.ImageFilePath = savedImagePaths.OriginalPath;
        existPropertyImage.ThumbnailFilePath = savedImagePaths.ThumbnailPath;
        existPropertyImage.PropertyId = updateDTO.PropertyId != existPropertyImage.PropertyId ? updateDTO.PropertyId : existPropertyImage.PropertyId;

        await _repository.UpdateAsync(existPropertyImage).ConfigureAwait(false);
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
