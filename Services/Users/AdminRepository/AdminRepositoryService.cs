using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Authorization;
using RealEstate.Data;
using RealEstate.Models.Users;
using RealEstate.Services.Images;

namespace RealEstate.Services.Users.AdminRepository
{
#pragma warning disable CA1515
    public class AdminRepositoryService(UserIdentityDbContext userIdentityContext,
                                        UserManager<User> userManager,
                                        ImageService imageService)
    {
        private readonly UserIdentityDbContext _userIdentityContext = userIdentityContext;
        private readonly ImageService _imageService = imageService;
        private readonly UserManager<User> _userManager = userManager;


        /// <summary>
        /// This function return all registered users
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUsersAsync() => [.. await _userManager.Users.AsNoTracking().Include(user => user.ProfileImage).ToListAsync().ConfigureAwait(false)];

        /// <summary>
        /// This function delete all users
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync().ConfigureAwait(false);
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, Roles.User).ConfigureAwait(false))
                    await _userManager.DeleteAsync(user).ConfigureAwait(false);
            }
            var environmentPath = _imageService.GetLocalImagesFullPath("auth");
            if (Directory.Exists(environmentPath))
            {
                var files = Directory.GetFiles(environmentPath);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }

        public async Task<IdentityResult> AssignRole(User user)
        {
            var allUsers = await GetUsersAsync().ConfigureAwait(false);
            bool userFlagged = false;
            if (allUsers.Count() == 1)
                userFlagged = true;
            return await _userManager.AddToRoleAsync(user, userFlagged ? Roles.Admin : Roles.User).ConfigureAwait(false);
        }

        public async Task<IdentityResult> PromoteUser(User user) {
            var result = await _userManager.AddToRoleAsync(user, Roles.Admin).ConfigureAwait(false);
            if (!result.Succeeded)
                return IdentityResult.Failed();
            return await _userManager.RemoveFromRoleAsync(user,Roles.User).ConfigureAwait(false);
        }
        

        public async Task<List<UserProfileImage>> GetUserProfileImageListAsync() =>
      await _userIdentityContext
     .UserProfileImages
     .AsNoTracking()
     .OrderByDescending(userProfileImg => userProfileImg.Id)
     .ToListAsync()
     .ConfigureAwait(false);

        public async Task<bool> IsAdmin(User user) => await _userManager.IsInRoleAsync(user, Roles.Admin).ConfigureAwait(false);


    }
}
