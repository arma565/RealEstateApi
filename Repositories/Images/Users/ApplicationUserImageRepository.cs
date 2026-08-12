using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Images.Users;

namespace RealEstate.Repositories.Images.Users;


interface IAgentImageRepository
{
    Task<IEnumerable<ApplicationUserImage>> GetListAsync();
    Task<ApplicationUserImage?> GetAsync(Guid id);
    Task<ApplicationUserImage> AddAsync(ApplicationUserImage agentImage);
    Task UpdateAsync(ApplicationUserImage agentImage);
    Task DeleteAsync(ApplicationUserImage agentImage);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class ApplicationUserImageRepository(AppDbContext context) : IAgentImageRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<ApplicationUserImage>> GetListAsync() =>
         await _context
            .AgentImages
            .AsNoTracking()
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<ApplicationUserImage?> GetAsync(Guid id) =>
            await _context
            .AgentImages
            .AsNoTracking()
            .SingleOrDefaultAsync(image => image.Id == id)
            .ConfigureAwait(false);

    public async Task<ApplicationUserImage> AddAsync(ApplicationUserImage agentImage)
    {
        await _context.AgentImages.AddAsync(agentImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return agentImage;
    }

    public async Task UpdateAsync(ApplicationUserImage agentImage)
    {
        _context.AgentImages.Update(agentImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(ApplicationUserImage agentImage)
    {
        _context.AgentImages.Remove(agentImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        await _context.AgentImages.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}
