using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Entities.Users;
using RealEstate.Enums.Users.Authentications;

namespace RealEstate.Repositories.Users;

interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetUsersListAsync();
    Task<ApplicationUser?> GetAsync(string userId);
    Task<ApplicationUser?> GetByUserNameAsync(string userName);
    Task<IdentityResult> RegisterAsync(ApplicationUser applicationUser, string password);
    Task<SignInResult> LoginAsync(string userName, string password);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser applicationUser, string token, string newPassword);
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser applicationUser, string currentPassword, string newPassword);
    Task<IdentityResult> EditUserProfileAsync(ApplicationUser applicationUser);
    Task<IdentityResult> DeleteAsync(ApplicationUser applicationUser);
    Task<IdentityResult> AssignAsync(ApplicationUser applicationUser);
    Task<IdentityResult> PromoteAsync(ApplicationUser user);
    Task<IdentityResult> DemoteAsync(ApplicationUser user);
    Task<bool> IsAdmin(ApplicationUser user);
    Task<bool> IsManager(ApplicationUser user);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByUsernameAsync(string userName);
    Task<bool> IsEmailConfirmedAsync(ApplicationUser applicationUser);

}

#pragma warning disable CA1515
public class UserRepository(UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager) : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    public async Task<IEnumerable<ApplicationUser>> GetUsersListAsync() =>
         await _userManager
             .Users
             .AsNoTracking()
             .Include(user => user.AgentImage)
             .Include(user => user.RealEstateProperties)
             .Include(user => user.RefreshToken)
             .ToListAsync().ConfigureAwait(false);

    public async Task<ApplicationUser?> GetAsync(string userId) =>
         await _userManager
             .Users
             .AsNoTracking()
             .Include(user => user.AgentImage)
             .Include(user => user.RealEstateProperties)
             .Include(user => user.RefreshToken)
             .SingleOrDefaultAsync(user => user.Id == userId)
             .ConfigureAwait(false);

    public async Task<ApplicationUser?> GetByUserNameAsync(string userName) =>
         await _userManager
             .Users
             .AsNoTracking()
             .Include(user => user.AgentImage)
             .Include(user => user.RealEstateProperties)
             .Include(user => user.RefreshToken)
             .SingleOrDefaultAsync(user => user.UserName == userName)
             .ConfigureAwait(false);

    public async Task<IdentityResult> RegisterAsync(ApplicationUser applicationUser, string password) =>
         await _userManager.CreateAsync(applicationUser, password).ConfigureAwait(false);

    public async Task<SignInResult> LoginAsync(string userName, string password) =>
         await _signInManager.PasswordSignInAsync(
           userName,
           password,
           false,
           false
        ).ConfigureAwait(false);

    public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser applicationUser, string token, string newPassword) =>
         await _userManager.ResetPasswordAsync(applicationUser, token, newPassword).ConfigureAwait(false);

    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser applicationUser, string currentPassword, string newPassword) =>
         await _userManager.ChangePasswordAsync(applicationUser, currentPassword, newPassword).ConfigureAwait(false);

    public async Task<IdentityResult> EditUserProfileAsync(ApplicationUser applicationUser) =>
         await _userManager.UpdateAsync(applicationUser).ConfigureAwait(false);

    public async Task<IdentityResult> DeleteAsync(ApplicationUser applicationUser) =>
         await _userManager.DeleteAsync(applicationUser).ConfigureAwait(false);

    public async Task<IdentityResult> AssignAsync(ApplicationUser applicationUser) =>
         await _userManager.AddToRoleAsync(applicationUser, Roles.Agent.ToString()).ConfigureAwait(false);

    public async Task<IdentityResult> PromoteAsync(ApplicationUser user)
    {
        var result = await _userManager.AddToRoleAsync(user, Roles.Admin.ToString()).ConfigureAwait(false);
        if (!result.Succeeded)
            return IdentityResult.Failed();
        return await _userManager.RemoveFromRoleAsync(user, Roles.Agent.ToString()).ConfigureAwait(false);
    }

    public async Task<IdentityResult> DemoteAsync(ApplicationUser user)
    {
        var result = await _userManager.AddToRoleAsync(user, Roles.Agent.ToString()).ConfigureAwait(false);
        if (!result.Succeeded)
            return IdentityResult.Failed();
        return await _userManager.RemoveFromRoleAsync(user, Roles.Admin.ToString()).ConfigureAwait(false);
    }

    public async Task<bool> IsAdmin(ApplicationUser user) =>
         await _userManager.IsInRoleAsync(user, Roles.Admin.ToString()).ConfigureAwait(false);

    public async Task<bool> IsManager(ApplicationUser user) =>
       await _userManager.IsInRoleAsync(user, Roles.Manager.ToString()).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByEmailAsync(string email) =>
         await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByUsernameAsync(string userName) =>
         await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

    public async Task<bool> IsEmailConfirmedAsync(ApplicationUser applicationUser) =>
         await _userManager.IsEmailConfirmedAsync(applicationUser).ConfigureAwait(false);


}
