using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Authorization;
using RealEstate.Entities.Users;

namespace RealEstate.Repositories.Users;

interface IAdminRepository
{
    Task<IEnumerable<ApplicationUser>> GetUsersListAsync();
    Task DeleteUserAsync(ApplicationUser user);
    Task<IdentityResult> PromoteAsync(ApplicationUser user);
    Task<bool> IsAdmin(ApplicationUser user);
}

#pragma warning disable CA1515
public class AdminRepository(UserManager<ApplicationUser> userManager) : IAdminRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IEnumerable<ApplicationUser>> GetUsersListAsync() =>
        [.. await _userManager.Users.AsNoTracking().Include(user => user.ProfileImage).ToListAsync().ConfigureAwait(false)];

    public async Task DeleteUserAsync(ApplicationUser user) =>
         await _userManager.DeleteAsync(user).ConfigureAwait(false);

    public async Task<IdentityResult> PromoteAsync(ApplicationUser user) {
        var result = await _userManager.AddToRoleAsync(user, Roles.Admin).ConfigureAwait(false);
        if (!result.Succeeded)
            return IdentityResult.Failed();
        return await _userManager.RemoveFromRoleAsync(user, Roles.Agent).ConfigureAwait(false);
    }

    public async Task<bool> IsAdmin(ApplicationUser user) => 
        await _userManager.IsInRoleAsync(user, Roles.Admin).ConfigureAwait(false);

}
