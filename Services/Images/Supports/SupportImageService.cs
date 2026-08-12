using RealEstate.DTOs.Images.Supports;
using RealEstate.Entities.Images.Supports;
using RealEstate.Repositories.Images.Supports;

namespace RealEstate.Services.Images.Supports;


interface ISupportImageService
{
    Task<IEnumerable<SupportImage>> GetListAsync();
    Task<SupportImage> GetAsync(Guid id);
    Task<DownloadPaths> GetPathAsync(Guid id);
    Task<SupportImage> AddAsync(CreateDTO createDTO, IFormFile image);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO, IFormFile image);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class SupportImageService(SupportImageRepository
                                        repository, ImageProccess imageProccess) : ISupportImageService
{
    private readonly SupportImageRepository _repository = repository;

    private readonly ImageProccess _imageProccess = imageProccess;

    public async Task<IEnumerable<SupportImage>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<SupportImage> GetAsync(Guid id)
    {

        var supportImage = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(supportImage);

        return supportImage;
    }


    public async Task<SupportImage> AddAsync(CreateDTO createDTO, IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(image);
        var savedImagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new SupportImage()
        {
            ImageFilePath = savedImagePaths.OriginalPath,
            ThumbnailFilePath = savedImagePaths.ThumbnailPath,
            SupportId = createDTO.SupportId
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO, IFormFile image)
    {
        var existSupportImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(existSupportImage);
        await _imageProccess.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = existSupportImage.ImageFilePath,
            ThumbnailPath = existSupportImage.ThumbnailFilePath
        }).ConfigureAwait(false);


        ArgumentNullException.ThrowIfNull(updateDTO);

        ArgumentNullException.ThrowIfNull(image);
        var savedImagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);

        existSupportImage.ImageFilePath = savedImagePaths.OriginalPath;
        existSupportImage.ThumbnailFilePath = savedImagePaths.ThumbnailPath;
        existSupportImage.SupportId = existSupportImage.SupportId != updateDTO.SupportId ? updateDTO.SupportId : existSupportImage.SupportId;

        await _repository.UpdateAsync(existSupportImage).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var supportImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(supportImage);

        await _imageProccess.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = supportImage.ImageFilePath,
            ThumbnailPath = supportImage.ThumbnailFilePath
        }).ConfigureAwait(false);
        await _repository.DeleteAsync(supportImage).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var supportImages = await GetListAsync().ConfigureAwait(false);

        foreach (var supportImage in supportImages)
        {
            await _imageProccess.DeleteFilesAsync(new ImagePaths
            {
                OriginalPath = supportImage.ImageFilePath,
                ThumbnailPath = supportImage.ThumbnailFilePath
            }).ConfigureAwait(false);
        }
        await _repository.DeleteAllAsync().ConfigureAwait(false);
    }

    public async Task<DownloadPaths> GetPathAsync(Guid id)
    {
        var supportImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(supportImage);

        return await _imageProccess.GetPathsAsync(new ImagePaths
        {
            OriginalPath = supportImage.ImageFilePath,
            ThumbnailPath = supportImage.ThumbnailFilePath
        }).ConfigureAwait(false);
    }
}
