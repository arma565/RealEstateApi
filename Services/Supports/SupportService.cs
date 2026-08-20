using RealEstate.DTOs.Supports;
using RealEstate.Entities.Supports;
using RealEstate.Repositories.Supports;

namespace RealEstate.Services.Supports;

interface ISupportService
{
    Task<IEnumerable<RealEstateSupport>> GetListAsync();
    Task<RealEstateSupport> GetAsync(Guid id);
    Task<RealEstateSupport> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(Guid id , UpdateDTO updateDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class SupportService(SupportRepository<RealEstateSupport> repository) : ISupportService
{
    private readonly SupportRepository<RealEstateSupport> _repository = repository;

    public async Task<IEnumerable<RealEstateSupport>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<RealEstateSupport> GetAsync(Guid id)
    {
        var support = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(support);

        return support;
    }

    public async Task<RealEstateSupport> AddAsync(CreateDTO createDTO)
    {
        ArgumentNullException.ThrowIfNull(createDTO);

        return await _repository.AddAsync(new RealEstateSupport
        {
            Title = createDTO.Title,
            DetailsTitle = createDTO.DetailsTitle,
            DetailsSubtitle = createDTO.DetailsSubtitle
        }).ConfigureAwait(false); ;
    }

    public async Task UpdateAsync(Guid id , UpdateDTO updateDTO)
    {
        var support = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(support);

        ArgumentNullException.ThrowIfNull(updateDTO);

        support.Title = string.IsNullOrEmpty(updateDTO.Title) ? support.Title : updateDTO.Title;
        support.DetailsTitle = string.IsNullOrEmpty(updateDTO.DetailsTitle) ? support.DetailsTitle : updateDTO.DetailsTitle;
        support.DetailsSubtitle = string.IsNullOrEmpty(updateDTO.DetailsSubtitle) ? support.DetailsSubtitle : updateDTO.DetailsSubtitle;

        await _repository.UpdateAsync(support).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var support = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(support);

        await _repository.DeleteAsync(support).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
        await _repository.DeleteAllAsync().ConfigureAwait(false);
    

}



