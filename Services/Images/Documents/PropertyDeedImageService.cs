using RealEstate.DTOs.Images.Documents;
using RealEstate.Entities.Images.Documents;
using RealEstate.Repositories.Images.Documents;

namespace RealEstate.Services.Images.Documents;
interface IPropertyDeedImageService
{
    Task<IEnumerable<PropertyDeedImage>> GetListAsync();
    Task<PropertyDeedImage> GetAsync(Guid id);
    Task<string> GetPathAsync(Guid id, bool isThumbnail);
    Task<PropertyDeedImage> AddAsync(CreateDTO createDTO , IFormFile image);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO, IFormFile image);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PropertyDeedImageService(PropertyDeedImageRepository<PropertyDeedImage>
                                        repository, ImageProccess imageProccess) : IPropertyDeedImageService
{
    private readonly PropertyDeedImageRepository<PropertyDeedImage> _repository = repository;

    private readonly ImageProccess _imageProccess = imageProccess;

    public async Task<IEnumerable<PropertyDeedImage>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<PropertyDeedImage> GetAsync(Guid id){
        var propertyDeed = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyDeed);

        return propertyDeed;
    }

    public async Task<string> GetPathAsync(Guid id, bool isThumbnail)
    {
        var propertyDeedImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(propertyDeedImage);

        return await _imageProccess.GetFullPathsAsync(isThumbnail ? propertyDeedImage.ThumbnailFilePath : propertyDeedImage.ImageFilePath).ConfigureAwait(false); ;
    }

    public async Task<PropertyDeedImage> AddAsync(CreateDTO createDTO, IFormFile image)
    {

        ArgumentNullException.ThrowIfNull(image);
        var savedImagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new PropertyDeedImage
        {
            ImageFilePath = savedImagePaths.OriginalPath,
            ThumbnailFilePath = savedImagePaths.ThumbnailPath,
            PropertyDeedId = createDTO.PropertyDeedId
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO, IFormFile image)
    {
        var existPropertyDeedImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(existPropertyDeedImage);

        await _imageProccess.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = existPropertyDeedImage.ImageFilePath,
            ThumbnailPath = existPropertyDeedImage.ThumbnailFilePath
        }).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(updateDTO);

        ArgumentNullException.ThrowIfNull(image);
        var savedImagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);

        existPropertyDeedImage.ImageFilePath = savedImagePaths.OriginalPath;
        existPropertyDeedImage.ThumbnailFilePath = savedImagePaths.ThumbnailPath;
        existPropertyDeedImage.PropertyDeedId = updateDTO.PropertyDeedId != existPropertyDeedImage.PropertyDeedId ? updateDTO.PropertyDeedId : existPropertyDeedImage.PropertyDeedId;

        await _repository.UpdateAsync(existPropertyDeedImage).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var propertyDeedImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(propertyDeedImage);

        await _imageProccess.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = propertyDeedImage.ImageFilePath,
            ThumbnailPath = propertyDeedImage.ThumbnailFilePath
        }).ConfigureAwait(false);

        await _repository.DeleteAsync(propertyDeedImage).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var propertyDeedImages = await GetListAsync().ConfigureAwait(false);

        foreach (var propertyDeedImage in propertyDeedImages)
        {
            await _imageProccess.DeleteFilesAsync(new ImagePaths
            {
                OriginalPath = propertyDeedImage.ImageFilePath,
                ThumbnailPath = propertyDeedImage.ThumbnailFilePath
            }).ConfigureAwait(false);
        }
        await _repository.DeleteAllAsync().ConfigureAwait(false);
    }
}
