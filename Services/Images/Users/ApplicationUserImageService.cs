using RealEstate.DTOs.Images.Users;
using RealEstate.Entities.Images.Supports;
using RealEstate.Entities.Images.Users;
using RealEstate.Repositories.Images.Users;

namespace RealEstate.Services.Images.Users;


interface IApplicationUserImageImageService
{
    Task<IEnumerable<ApplicationUserImage>> GetListAsync();
    Task<ApplicationUserImage> GetAsync(Guid id);
    Task<DownloadPaths> GetPathAsync(Guid id);
    Task<ApplicationUserImage> AddAsync(CreateDTO createDTO, IFormFile image);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO, IFormFile image);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class ApplicationUserImageImageService(ApplicationUserImageRepository
                                        repository, ImageProccess imageProccess) : IApplicationUserImageImageService
{
    private readonly ApplicationUserImageRepository _repository = repository;

    private readonly ImageProccess _imageProccess = imageProccess;

    public async Task<IEnumerable<ApplicationUserImage>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<ApplicationUserImage> GetAsync(Guid id)
    {
        var applicationUserImage = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(applicationUserImage);

        return applicationUserImage;
    }

    public async Task<ApplicationUserImage> AddAsync(CreateDTO createDTO, IFormFile image)
    {
        ArgumentNullException.ThrowIfNull(image);
        var savedImagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new ApplicationUserImage
        {
            ImageFilePath = savedImagePaths.OriginalPath,
            ThumbnailFilePath = savedImagePaths.ThumbnailPath,
            AgentId = createDTO.AgentId
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO, IFormFile image)
    {
        var existAgentImage = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(existAgentImage);
        await _imageProccess.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = existAgentImage.ImageFilePath,
            ThumbnailPath = existAgentImage.ThumbnailFilePath
        }).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(updateDTO);

        var savedImagePaths = await _imageProccess.SaveAsync(image).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(savedImagePaths);

        existAgentImage.ImageFilePath = savedImagePaths.OriginalPath;
        existAgentImage.ThumbnailFilePath = savedImagePaths.ThumbnailPath;
        existAgentImage.AgentId = updateDTO.AgentId != existAgentImage.AgentId ? updateDTO.AgentId : existAgentImage.AgentId;

        await _repository.UpdateAsync(existAgentImage).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var agentImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(agentImage);

        await _imageProccess.DeleteFilesAsync(new ImagePaths
        {
            OriginalPath = agentImage.ImageFilePath,
            ThumbnailPath = agentImage.ThumbnailFilePath
        }).ConfigureAwait(false);
        await _repository.DeleteAsync(agentImage).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var agentImages = await GetListAsync().ConfigureAwait(false);

        foreach (var agentImage in agentImages)
        {
            await _imageProccess.DeleteFilesAsync(new ImagePaths
            {
                OriginalPath = agentImage.ImageFilePath,
                ThumbnailPath = agentImage.ThumbnailFilePath
            }).ConfigureAwait(false);
        }
        await _repository.DeleteAllAsync().ConfigureAwait(false);
    }

    public async Task<DownloadPaths> GetPathAsync(Guid id)
    {
        var agentImage = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(agentImage);

        return await _imageProccess.GetPathsAsync(new ImagePaths
        {
            OriginalPath = agentImage.ImageFilePath,
            ThumbnailPath = agentImage.ThumbnailFilePath
        }).ConfigureAwait(false);
    }
}
