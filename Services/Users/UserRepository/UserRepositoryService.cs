using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Authorization;
using RealEstate.Data;
using RealEstate.Models.Users;
using RealEstate.Services.Images;

namespace RealEstate.Services.Users.UserRepository
{
#pragma warning disable CA1515
    public class UserRepositoryService(UserIdentityDbContext userIdentityContext,
                                        UserManager<User> userManager,
                                        SignInManager<User> signInManager,
                                        ImageService imageService)
    {

        private readonly UserIdentityDbContext _userIdentityContext = userIdentityContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly ImageService _imageService = imageService;

        #region UserServices

        /// <summary>
        /// This function return a user using id
        /// </summary>
        /// <returns> User associated to user ID </returns>
        public async Task<User?> GetUserByIDAsync(string userID) =>
             await _userManager
                 .Users.AsNoTracking()
                 .Include(user => user.ProfileImage)
                 .SingleOrDefaultAsync(user => user.Id == userID)
                 .ConfigureAwait(false);

        /// <summary>
        /// This function return a user using userName
        /// </summary>
        /// <returns> User associated to userName </returns>
        public async Task<User?> GetUserByUserNameAsync(string userName) =>
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

            return await _userManager.CreateAsync(new User
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
        public async Task<IdentityResult> DeleteUserAsync(User user)
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
        public async Task<string> GenerateTokenToRecoverUserAsync(User user) => await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
        
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
            User user,
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
        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
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
        public async Task<IdentityResult> EditUserProfileAsync(User editUser) => await _userManager.UpdateAsync(editUser).ConfigureAwait(false);

        public async Task<User?> FindUserByEmailAsync(string email) => await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

        public async Task<User?> FindUserByUserNameAsync(string userName) => await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

        public async Task<User?> FindUserByIDAsync(string userId) => await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

        public async Task<bool> IsEmailConfirmed(User user) => await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false);

        #endregion

        #region UserProfileImageServices

        public async Task<UserProfileImage?> GetProfileImageAsync(Guid userProfileImageID) =>
       await _userIdentityContext
      .UserProfileImages.AsNoTracking()
      .SingleOrDefaultAsync(userProfileImg => userProfileImg.Id == userProfileImageID)
      .ConfigureAwait(false);

        public async Task AddProfileImageAsync(UserProfileImage userProfileImage)
        {
            await _userIdentityContext.UserProfileImages.AddAsync(userProfileImage).ConfigureAwait(false);
            await _userIdentityContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateProfileImageAsync(UserProfileImage profileImage)
        {
            _userIdentityContext.UserProfileImages.Update(profileImage);
            await _userIdentityContext.SaveChangesAsync().ConfigureAwait(false);
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
            _userIdentityContext.UserProfileImages.Remove(userProfileImage);
            await _userIdentityContext.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion
    }
}
