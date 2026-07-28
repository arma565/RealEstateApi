using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Models.Users;
using RealEstate.Services.Images;

namespace RealEstate.Services.Repositories.Users.UserRepository;

interface IUserRepository
{
    Task<ApplicationUser?> GetByIDAsync(string userId);
    Task<ApplicationUser?> GetByUserNameAsync(string userName);
    Task<IdentityResult> RegisterAsync(UserRegisterAccount userRegister);
    Task<SignInResult> LoginAsync(UserLoginRequest userRequest);
    Task<IdentityResult> DeleteAsync(ApplicationUser user);
    Task<string> GenerateTokenToRecoverUserAsync(ApplicationUser user);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
    Task<IdentityResult> EditUserProfileAsync(ApplicationUser editUser);
    Task<ApplicationUser?> FindUserByEmailAsync(string email);
    Task<ApplicationUser?> FindUserByUserNameAsync(string userName);
    Task<ApplicationUser?> FindUserByIDAsync(string userId);
    Task<bool> IsEmailConfirmed(ApplicationUser user);
}

#pragma warning disable CA1515
public class UserRepository(AppDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    SignInManager<ApplicationUser> signInManager,
                                    ImageService imageService) : IUserRepository
{

    private readonly AppDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ImageService _imageService = imageService;

    #region UserServices

    /// <summary>
    /// This function return a user using id
    /// </summary>
    /// <returns> User associated to user ID </returns>
    public async Task<ApplicationUser?> GetUserByIDAsync(string userID) =>
         await _userManager
             .Users.AsNoTracking()
             .Include(user => user.ProfileImage)
             .SingleOrDefaultAsync(user => user.Id == userID)
             .ConfigureAwait(false);

    /// <summary>
    /// This function return a user using userName
    /// </summary>
    /// <returns> User associated to userName </returns>
    public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName) =>
         await _userManager
             .Users
             .AsNoTracking()
             .Include(user => user.ProfileImage)
             .SingleOrDefaultAsync(user => user.UserName == userName)
             .ConfigureAwait(false);

    /// <summary>
    /// This function register a user in database
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RegisterUserAsync(UserRegisterAccount userRegister)
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

    /// <summary>
    /// Login user using username and password
    /// </summary>
    /// <param name="model">
    /// Login model containing username and password
    /// </param>
    /// <returns></returns>
    public async Task<SignInResult> LoginUserAsync(UserLoginRequest userRequest)
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

    /// <summary>
    /// This function Delete a user from identity store
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
    {
        if (user?.ProfileImage != null && user.ProfileImage.ProfileImageName != null)
        {
            var environmentPath = _imageService.GetLocalImagesFullPath("auth");

            var filePath = Path.Combine(environmentPath, user.ProfileImage.ProfileImageName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        return await _userManager.DeleteAsync(user!).ConfigureAwait(false);
    }

    /// <summary>
    /// This function create a token to reset password
    /// </summary>
    /// <param name="user">
    /// User account which needs reset
    /// </param>
    /// <returns></returns>
    public async Task<string> GenerateTokenToRecoverUserAsync(ApplicationUser user) => await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

    /// <summary>
    /// Reset the user account password
    /// </summary>
    /// <param name="user">
    /// user account
    /// </param>
    /// <param name="token">
    /// Tokeen reset password
    /// </param>
    /// <param name="newPassword">
    /// new password of account
    /// </param>
    /// <returns></returns>
    public async Task<IdentityResult> ResetPasswordAsync(
        ApplicationUser user,
        string token,
        string newPassword
    ) => await _userManager.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false);

    /// <summary>
    /// This function change the user account password using current password and new password
    /// </summary>
    /// <param name="user">
    /// User account
    /// </param>
    /// <param name="currentPassword">
    /// Current password of the account
    /// </param>
    /// <param name="newPassword">
    /// New password for the account
    /// </param>
    /// <returns></returns>
    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);
    }

    /// <summary>
    /// This function is useful to edit profile
    /// </summary>
    /// <param name="user">
    /// user account
    /// </param>
    /// <returns></returns>
    public async Task<IdentityResult> EditUserProfileAsync(ApplicationUser editUser) => await _userManager.UpdateAsync(editUser).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindUserByEmailAsync(string email) => await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindUserByUserNameAsync(string userName) => await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindUserByIDAsync(string userId) => await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

    public async Task<bool> IsEmailConfirmed(ApplicationUser user) => await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false);

    #endregion

    #region UserProfileImageServices

    public async Task<UserProfileImage?> GetProfileImageAsync(Guid userProfileImageID) =>
   await _context
  .UserProfileImages.AsNoTracking()
  .SingleOrDefaultAsync(userProfileImg => userProfileImg.Id == userProfileImageID)
  .ConfigureAwait(false);

    public async Task AddProfileImageAsync(UserProfileImage userProfileImage)
    {
        await _context.UserProfileImages.AddAsync(userProfileImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateProfileImageAsync(UserProfileImage profileImage)
    {
        _context.UserProfileImages.Update(profileImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteProfileImageAsync(UserProfileImage userProfileImage)
    {
        if (userProfileImage == null || userProfileImage.ProfileImageName == null)
            return;
        var environmentPath = _imageService.GetLocalImagesFullPath("auth");
        var profileImagePath = Path.Combine(environmentPath, userProfileImage.ProfileImageName);
        var filesDir = Directory.GetFiles(environmentPath);
        foreach (var filePath in filesDir)
        {
            if (filePath == profileImagePath)
                File.Delete(filePath);
        }
        _context.UserProfileImages.Remove(userProfileImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
    #endregion
}
