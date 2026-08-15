using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Authorization;
using RealEstate.Entities.Users;

namespace RealEstate.Repositories.Users;

interface IAdminRepository
{
    Task<IEnumerable<ApplicationUser>> GetUsersListAsync();
    Task<ApplicationUser?> GetAsync(string userId);
    Task<ApplicationUser?> GetByUserNameAsync(string userName);
    Task<IdentityResult> RegisterAsync(ApplicationUser applicationUser, string password);
    Task AssignAsync(string userName);
    Task<IdentityResult> PromoteAsync(ApplicationUser user);
    Task DeleteUserAsync(ApplicationUser user);
    Task<bool> IsAdmin(ApplicationUser user);
}

#pragma warning disable CA1515
public class AdminRepository(UserManager<ApplicationUser> userManager) : IAdminRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IEnumerable<ApplicationUser>> GetUsersListAsync() =>
        await _userManager
        .Users
        .AsNoTracking()
        .Include(user => user.AgentImage)
        .Include(user => user.RealEstateProperties)
        .ToListAsync().ConfigureAwait(false);

    public async Task<ApplicationUser?> GetAsync(string userId) =>
     await _userManager
         .Users
         .AsNoTracking()
         .Include(user => user.AgentImage)
         .Include(user => user.RealEstateProperties)
         .SingleOrDefaultAsync(user => user.Id == userId)
         .ConfigureAwait(false);

    public async Task<ApplicationUser?> GetByUserNameAsync(string userName) =>
         await _userManager
             .Users
             .AsNoTracking()
             .Include(user => user.AgentImage)
             .Include(user => user.RealEstateProperties)
             .SingleOrDefaultAsync(user => user.UserName == userName)
             .ConfigureAwait(false);

    public async Task<IdentityResult> RegisterAsync(ApplicationUser applicationUser, string password) =>
     await _userManager.CreateAsync(applicationUser, password).ConfigureAwait(false);

    public async Task DeleteUserAsync(ApplicationUser user) =>
         await _userManager.DeleteAsync(user).ConfigureAwait(false);

    public async Task AssignAsync(string userName)
    {
        var user = await GetByUserNameAsync(userName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);
        await _userManager.AddToRoleAsync(user, Roles.Agent).ConfigureAwait(false);
    }
    
    public async Task<IdentityResult> PromoteAsync(ApplicationUser user)
    {
        var result = await _userManager.AddToRoleAsync(user, Roles.Admin).ConfigureAwait(false);
        if (!result.Succeeded)
            return IdentityResult.Failed();
        return await _userManager.RemoveFromRoleAsync(user, Roles.Agent).ConfigureAwait(false);
    }

    public async Task<bool> IsAdmin(ApplicationUser user) => 
        await _userManager.IsInRoleAsync(user, Roles.Admin).ConfigureAwait(false);

}
