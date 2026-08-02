using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Services.Images;
using RealEstate.Services.Models.Users;

namespace RealEstate.Services.Repositories.Users.UserRepositories;

interface IUserRepository
{
    Task<ApplicationUser?> GetByIDAsync(string userId);
    Task<ApplicationUser?> GetByUserNameAsync(string userName);
    Task<IdentityResult> RegisterAsync(UserRegisterAccountDTO userRegister);
    Task<SignInResult> LoginAsync(UserLoginRequestDTO userRequest);
    Task<IdentityResult> DeleteAsync(ApplicationUser applicationUser);
    Task<string> GenerateTokenToRecoverUserAsync(ApplicationUser applicationUser);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser applicationUser, string token, string newPassword);
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser applicationUser, string currentPassword, string newPassword);
    Task<IdentityResult> EditUserProfileAsync(ApplicationUser applicationUser);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByUserNameAsync(string userName);
    Task<ApplicationUser?> FindByIDAsync(string userId);
    Task<bool> IsEmailConfirmed(ApplicationUser applicationUser);
}

#pragma warning disable CA1515
public class UserRepository(
                                    UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    ImageService imageService) : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ImageService _imageService = imageService;

    public async Task<ApplicationUser?> GetByIDAsync(string userId) =>
         await _userManager
             .Users.AsNoTracking()
             .Include(user => user.ProfileImage)
             .SingleOrDefaultAsync(user => user.Id == userId)
             .ConfigureAwait(false);

    public async Task<ApplicationUser?> GetByUserNameAsync(string userName) =>
         await _userManager
             .Users
             .AsNoTracking()
             .Include(user => user.ProfileImage)
             .SingleOrDefaultAsync(user => user.UserName == userName)
             .ConfigureAwait(false);

    public async Task<IdentityResult> RegisterAsync(UserRegisterAccountDTO userRegister)
    {
        if (userRegister is null)
            return IdentityResult.Failed(new IdentityError
            {
                Code = "userRegister Null",
                Description = "Failed to retrieve parameter!"
            });

        return await _userManager.CreateAsync(new ApplicationUser
        {
            UserName = userRegister.UserName,
            Email = userRegister.Email,
            AcceptTerms = userRegister.AcceptTerms
        }, userRegister.Password).ConfigureAwait(false);

    }

    public async Task<SignInResult> LoginAsync(UserLoginRequestDTO userRequest)
    {
        if (userRequest is null)
            return SignInResult.Failed;

        return await _signInManager.PasswordSignInAsync(
           userRequest.UserName,
           userRequest.Password,
           false,
           false
        ).ConfigureAwait(false);
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser applicationUser)
    {
        var user = _userManager.Users.FirstOrDefault(aUser => aUser.Id == applicationUser.Id);

        if (user == null)
            ArgumentNullException.ThrowIfNull(user);

        await _imageService.DeleteImages([user.ProfileImage]).ConfigureAwait(false);

        return await _userManager.DeleteAsync(user).ConfigureAwait(false);
    }

    public async Task<string> GenerateTokenToRecoverUserAsync(ApplicationUser applicationUser) => await _userManager.GeneratePasswordResetTokenAsync(applicationUser).ConfigureAwait(false);

    public async Task<IdentityResult> ResetPasswordAsync(
        ApplicationUser applicationUser,
        string token,
        string newPassword
    ) => await _userManager.ResetPasswordAsync(applicationUser, token, newPassword).ConfigureAwait(false);

    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser applicationUser, string currentPassword, string newPassword) =>
        await _userManager.ChangePasswordAsync(applicationUser, currentPassword, newPassword).ConfigureAwait(false);

    public async Task<IdentityResult> EditUserProfileAsync(ApplicationUser applicationUser) => await _userManager.UpdateAsync(applicationUser).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByEmailAsync(string email) => await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByUserNameAsync(string userName) => await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByIDAsync(string userId) => await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

    public async Task<bool> IsEmailConfirmed(ApplicationUser applicationUser) => await _userManager.IsEmailConfirmedAsync(applicationUser).ConfigureAwait(false);
}
