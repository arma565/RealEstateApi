using RealEstate.Entities.Images;
using RealEstate.Repositories.Images;

namespace RealEstate.Services.Images;

interface IImageService
{
    Task<IEnumerable<RealEstateImage>> GetListAsync();
    Task<DownloadPaths> GetPathAsync(Guid id);
    Task<RealEstateImage?> GetAsync(Guid id);
    Task<RealEstateImage> AddAsync(RealEstateImageDTO realEstateImageDTO);
    Task UpdateAsync(Guid id, RealEstateImageDTO realEstateImageDTO, IFormFile image);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class ImageService(ImageRepository repository,
                                        ImageProccess imageProccess) : IImageService
{
    private readonly ImageRepository _repository = repository;

    private readonly ImageProccess _imageProccess = imageProccess;

    public async Task<IEnumerable<RealEstateImage>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<RealEstateImage?> GetAsync(Guid id) =>
        await _repository.GetAsync(id).ConfigureAwait(false);

    public async Task<DownloadPaths> GetPathAsync(Guid id)
    {
        var realEstateImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(realEstateImage);

        return await _imageProccess.GetPathsAsync(new ImagePaths
        {
            OriginalPath = realEstateImage.ImageFilePath,
            ThumbnailPath = realEstateImage.ThumbnailFilePath
        }).ConfigureAwait(false);
    }

    public async Task<RealEstateImage> AddAsync(RealEstateImageDTO realEstateImageDTO)
    {

        ArgumentNullException.ThrowIfNull(realEstateImageDTO);

        var realEstateImage = new RealEstateImage
        {
            UserId = realEstateImageDTO.UserId,
            PropertyDeedId = realEstateImageDTO.PropertyDeedId,
            SupportId = realEstateImageDTO.SupportId
        };

        return await _repository.AddAsync(realEstateImage).ConfigureAwait(false); ;
    }

    public async Task UpdateAsync(Guid id, RealEstateImageDTO realEstateImageDTO, IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(realEstateImageDTO);

        var realEstateImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(realEstateImage);

        var imagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(imagePaths);

        realEstateImage.Id = id;
        realEstateImage.ImageFilePath = imagePaths.OriginalPath;
        realEstateImage.ThumbnailFilePath = imagePaths.ThumbnailPath;
        realEstateImage.UserId = realEstateImageDTO.UserId;
        realEstateImage.PropertyDeedId = realEstateImageDTO.PropertyDeedId;
        realEstateImage.SupportId = realEstateImageDTO.SupportId;

        await _repository.UpdateAsync(realEstateImage).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {

        var realEstateImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(realEstateImage);

        await _imageProccess.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = realEstateImage.ImageFilePath,
            ThumbnailPath = realEstateImage.ThumbnailFilePath
        }).ConfigureAwait(false);

        await _repository.DeleteAsync(realEstateImage).ConfigureAwait(false);
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
}
