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
                                        UserManager<ApplicationUser> userManager,
                                        ImageService imageService)
    {
        private readonly UserIdentityDbContext _userIdentityContext = userIdentityContext;
        private readonly ImageService _imageService = imageService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;


        /// <summary>
        /// This function return all registered users
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync() => [.. await _userManager.Users.AsNoTracking().Include(user => user.ProfileImage).ToListAsync().ConfigureAwait(false)];

        /// <summary>
        /// This function delete all users
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync().ConfigureAwait(false);
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, Roles.Agent).ConfigureAwait(false))
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

        public async Task<IdentityResult> AssignRole(ApplicationUser user)
        {
            var allUsers = await GetUsersAsync().ConfigureAwait(false);
            bool userFlagged = false;
            if (allUsers.Count() == 1)
                userFlagged = true;
            return await _userManager.AddToRoleAsync(user, userFlagged ? Roles.Admin : Roles.Agent).ConfigureAwait(false);
        }

        public async Task<IdentityResult> PromoteUser(ApplicationUser user) {
            var result = await _userManager.AddToRoleAsync(user, Roles.Admin).ConfigureAwait(false);
            if (!result.Succeeded)
                return IdentityResult.Failed();
            return await _userManager.RemoveFromRoleAsync(user,Roles.Agent).ConfigureAwait(false);
        }
        

        public async Task<List<UserProfileImage>> GetUserProfileImageListAsync() =>
      await _userIdentityContext
     .UserProfileImages
     .AsNoTracking()
     .OrderByDescending(userProfileImg => userProfileImg.Id)
     .ToListAsync()
     .ConfigureAwait(false);

        public async Task<bool> IsAdmin(ApplicationUser user) => await _userManager.IsInRoleAsync(user, Roles.Admin).ConfigureAwait(false);


    }
}
