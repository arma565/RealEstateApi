using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Images.Users;

namespace RealEstate.Repositories.Images.Users;

#pragma warning disable CA1515
public class ApplicationUserImageRepository<TApplicationUserImage>(AppDbContext context) : BaseRepository<ApplicationUserImage>
{
    private readonly AppDbContext _context = context;

    public override async Task<IEnumerable<ApplicationUserImage>> GetListAsync() =>
         await _context
            .AgentImages
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

    public override async Task<ApplicationUserImage?> GetAsync(Guid id) =>
            await _context
            .AgentImages
            .AsNoTracking()
            .SingleOrDefaultAsync(image => image.Id == id)
            .ConfigureAwait(false);

    public override async Task<ApplicationUserImage> AddAsync(ApplicationUserImage agentImage)
    {
        await _context.AgentImages.AddAsync(agentImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return agentImage;
    }

    public override async Task UpdateAsync(ApplicationUserImage agentImage)
    {
        _context.AgentImages.Update(agentImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAsync(ApplicationUserImage agentImage)
    {
        _context.AgentImages.Remove(agentImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public override async Task DeleteAllAsync()
    {
        await _context.AgentImages.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
