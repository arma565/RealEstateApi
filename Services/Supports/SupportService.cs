using RealEstate.DTOs.Supports;
using RealEstate.Entities.Supports;
using RealEstate.Repositories.Supports;

namespace RealEstate.Services.Supports;

interface ISupportService
{
    Task<IEnumerable<RealEstateSupport>> GetListAsync();
    Task<RealEstateSupport?> GetAsync(Guid id);
    Task<RealEstateSupport> AddAsync(RealEstateSupportDTO realEstateSupportDTO);
    Task UpdateAsync(Guid id , RealEstateSupportDTO realEstateSupportDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class SupportService(SupportRepository support) : ISupportService
{
    private readonly SupportRepository _support = support;

    //private readonly ImageRepository _imageRepository = imageRepository;

    public async Task<IEnumerable<RealEstateSupport>> GetListAsync() =>
        await _support.GetListAsync().ConfigureAwait(false);

    public async Task<RealEstateSupport?> GetAsync(Guid id) =>
         await _support.GetAsync(id).ConfigureAwait(false);

    public async Task<RealEstateSupport> AddAsync(RealEstateSupportDTO realEstateSupportDTO)
    {

        ArgumentNullException.ThrowIfNull(realEstateSupportDTO);

        var realEstateSupport = new RealEstateSupport
        {
            Title = realEstateSupportDTO.Title,
            DetailsTitle = realEstateSupportDTO.DetailsTitle,
            DetailsSubtitle = realEstateSupportDTO.DetailsSubtitle,
            ImageId = realEstateSupportDTO.ImageId
        };

        return await _support.AddAsync(realEstateSupport).ConfigureAwait(false); ;
    }

    public async Task UpdateAsync(Guid id , RealEstateSupportDTO realEstateSupportDTO)
    {
        ArgumentNullException.ThrowIfNull(realEstateSupportDTO);

        var realEstateSupport = new RealEstateSupport
        {
            Id = id,
            Title = realEstateSupportDTO.Title,
            DetailsTitle = realEstateSupportDTO.DetailsTitle,
            DetailsSubtitle = realEstateSupportDTO.DetailsSubtitle,
            ImageId = realEstateSupportDTO.ImageId
        };

        await _support.UpdateAsync(realEstateSupport).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var support = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(support!.Image);

        //await _imageRepository.DeleteAsync(support.Image.Id).ConfigureAwait(false);

        await _support.DeleteAsync(support).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var supports = await GetListAsync().ConfigureAwait(false);

        //foreach (var support in supports)
        //{
        //    if (support == null || support.Image == null)
        //        continue;
        //    await _imageRepository.DeleteAsync(support.Image.Id).ConfigureAwait(false);
        //}

        await _support.DeleteAllAsync().ConfigureAwait(false);
    }

}



