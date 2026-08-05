using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Models.Supports;
using RealEstate.Services.Repositories.Images;


namespace RealEstate.Services.Repositories.Supports;

interface ISupportRepository
{
    Task<IEnumerable<RealEstateSupport>> GetListAsync();
    Task<RealEstateSupport?> GetAsync(Guid id);
    Task<RealEstateSupport> AddAsync(RealEstateSupportDTO realEstateSupportDTO);
    Task UpdateAsync(Guid id , RealEstateSupportDTO realEstateSupportDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class SupportRepository(AppDbContext context,
                                        ImageRepository imageRepository) : ISupportRepository
{
    private readonly AppDbContext _context = context;

    private readonly ImageRepository _imageRepository = imageRepository;

    public async Task<IEnumerable<RealEstateSupport>> GetListAsync() =>
       await _context
      .Supports
      .AsNoTracking()
      .Include(support => support.Image)
      .ToListAsync().ConfigureAwait(false);

    public async Task<RealEstateSupport?> GetAsync(Guid id) =>
      await _context
          .Supports.AsNoTracking()
          .Include(support => support.Image)
          .SingleOrDefaultAsync(support => support.Id == id)
          .ConfigureAwait(false);

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

        await _context.Supports.AddAsync(realEstateSupport).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return realEstateSupport;
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


        _context.Supports.Update(realEstateSupport);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var support = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(support!.Image);

        await _imageRepository.DeleteAsync(support.Image.Id).ConfigureAwait(false);

        _context.Supports.Remove(support);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var supports = await GetListAsync().ConfigureAwait(false);
        foreach (var support in supports)
        {
            if (support == null || support.Image == null)
                continue;
            await _imageRepository.DeleteAsync(support.Image.Id).ConfigureAwait(false);
        }
        await _context.Supports.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

}



