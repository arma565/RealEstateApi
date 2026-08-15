using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Entities.Users;

namespace RealEstate.Repositories.Users;

interface IUserRepository
{
    Task<SignInResult> LoginAsync(string userName, string password);
    Task<IdentityResult> DeleteAsync(ApplicationUser applicationUser);
    Task<string> GenerateTokenToRecoverUserAsync(ApplicationUser applicationUser);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser applicationUser, string token, string newPassword);
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser applicationUser, string currentPassword, string newPassword);
    Task<IdentityResult> EditUserProfileAsync(ApplicationUser applicationUser);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<bool> IsEmailConfirmedAsync(ApplicationUser applicationUser);
}

#pragma warning disable CA1515
public class UserRepository(UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager) : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    public async Task<SignInResult> LoginAsync(string userName, string password) =>
         await _signInManager.PasswordSignInAsync(
           userName,
           password,
           false,
           false
        ).ConfigureAwait(false);

    public async Task<IdentityResult> DeleteAsync(ApplicationUser applicationUser) =>
         await _userManager.DeleteAsync(applicationUser).ConfigureAwait(false);

    public async Task<string> GenerateTokenToRecoverUserAsync(ApplicationUser applicationUser) =>
        await _userManager.GeneratePasswordResetTokenAsync(applicationUser).ConfigureAwait(false);

    public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser applicationUser, string token, string newPassword) =>
        await _userManager.ResetPasswordAsync(applicationUser, token, newPassword).ConfigureAwait(false);

    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser applicationUser, string currentPassword, string newPassword) =>
        await _userManager.ChangePasswordAsync(applicationUser, currentPassword, newPassword).ConfigureAwait(false);

    public async Task<IdentityResult> EditUserProfileAsync(ApplicationUser applicationUser) =>
        await _userManager.UpdateAsync(applicationUser).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByEmailAsync(string email) =>
        await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

    public async Task<bool> IsEmailConfirmedAsync(ApplicationUser applicationUser) =>
        await _userManager.IsEmailConfirmedAsync(applicationUser).ConfigureAwait(false);
}
