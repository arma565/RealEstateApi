using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Services.Authorization;
using RealEstate.Services.Images;
using RealEstate.Services.Models.Images;
using RealEstate.Services.Models.Users;
using RealEstate.Services.Repositories.Images;

namespace RealEstate.Services.Repositories.Users.AdminRepositories;

interface IAdminRepository
{
    Task<IEnumerable<ApplicationUser>> GetUsersListAsync();
    Task DeleteUsersAsync();
    Task<IdentityResult> AssignUserRole(ApplicationUser user);
    Task<IdentityResult> PromoteUser(ApplicationUser user);
    Task<List<RealEstateImage>> GetUserProfileImageListAsync();
}

#pragma warning disable CA1515
public class AdminRepository(AppDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    ImageRepository imageRepository) : IAdminRepository
{
    private readonly AppDbContext _context = context;
    private readonly ImageRepository _imageRepository = imageRepository;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IEnumerable<ApplicationUser>> GetUsersListAsync() => [.. await _userManager.Users.AsNoTracking().Include(user => user.ProfileImage).ToListAsync().ConfigureAwait(false)];

    public async Task DeleteUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync().ConfigureAwait(false);
        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, Roles.Agent).ConfigureAwait(false))
            {
                await _imageRepository.DeleteAsync(user.ProfileImage.Id).ConfigureAwait(false);
                await _userManager.DeleteAsync(user).ConfigureAwait(false);
            }
        }
    }

    public async Task<IdentityResult> AssignUserRole(ApplicationUser user)
    {
        var allUsers = await GetUsersListAsync().ConfigureAwait(false);
        bool userFlagged = false;
        if (allUsers.Count() == 1)
            userFlagged = true;
        return await _userManager.AddToRoleAsync(user, userFlagged ? Roles.Admin : Roles.Agent).ConfigureAwait(false);
    }

    public async Task<IdentityResult> PromoteUser(ApplicationUser user)
    {
        var result = await _userManager.AddToRoleAsync(user, Roles.Admin).ConfigureAwait(false);
        if (!result.Succeeded)
            return IdentityResult.Failed();
        return await _userManager.RemoveFromRoleAsync(user, Roles.Agent).ConfigureAwait(false);
    }

    public async Task<List<RealEstateImage>> GetUserProfileImageListAsync() =>
          await _context
         .Images
         .AsNoTracking()
         .OrderByDescending(userProfileImg => userProfileImg.Id)
         .ToListAsync()
         .ConfigureAwait(false);

    public async Task<bool> IsAdmin(ApplicationUser user) => await _userManager.IsInRoleAsync(user, Roles.Admin).ConfigureAwait(false);

}
