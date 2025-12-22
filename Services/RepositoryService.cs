using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Data;
using RealEstate.Models.Authentication;
using RealEstate.Models.Authentication.Users;
using RealEstate.Models.Estate;
using RealEstate.Models.Estate.Assets;
using RealEstate.Models.Support;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

#pragma warning disable CA1515
namespace RealEstate.Services
{
    public sealed class RepositoryService(AppDbContext context,
                                            UserManager<User> userManager,
                                            SignInManager<User> signInManager,
                                            ImageService imageService)
    {
        private readonly AppDbContext _context = context;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly ImageService _imageService = imageService;

        #region Authentication

        /// <summary>
        /// This function return all registered users
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsers() => [.. await _userManager.Users.AsNoTracking().Include(user => user.ProfileImage).ToListAsync().ConfigureAwait(false)];

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
                 .Users.AsNoTracking()
                 .Include(user => user.ProfileImage)
                 .SingleOrDefaultAsync(user => user.UserName == userName)
                 .ConfigureAwait(false);

        /// <summary>
        /// This function register a user in database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RegisterUserAsync(Register userRegister)
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
        /// This function delete all users
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync().ConfigureAwait(false);
           foreach (var user in users)
            {
                await _userManager.DeleteAsync(user).ConfigureAwait(false);
            }
            var environmentPath = _imageService.GetLocalImagesFullPath("auth");
            if (Directory.Exists(environmentPath)) {
                var files = Directory.GetFiles(environmentPath);
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// This function Delete a user from identity store
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            if (user?.ProfileImage != null && user.ProfileImage.ProfileImageName != null) {
                var environmentPath = _imageService.GetLocalImagesFullPath("auth");

                var filePath = Path.Combine(environmentPath, user.ProfileImage.ProfileImageName);

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            return await _userManager.DeleteAsync(user!).ConfigureAwait(false);
        }

        /// <summary>
        /// Login user using username and password
        /// </summary>
        /// <param name="model">
        /// Login model containing username and password
        /// </param>
        /// <returns></returns>
        public async Task<SignInResult> LoginUserAsync(Login model)
        {
            if (model is null)
            {
                return SignInResult.Failed;
            }
            return await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                false,
                false
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// This function create a token to reset password
        /// </summary>
        /// <param name="user">
        /// User account which needs reset
        /// </param>
        /// <returns></returns>
        public async Task<string> GenerateTokenToRecoverUserAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
        }

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
        )
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false);
        }

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
        public async Task<IdentityResult> EditUserProfileAsync(User updateUser) => await _userManager.UpdateAsync(updateUser).ConfigureAwait(false);

        public async Task<User?> FindUserByEmailAsync(string email) => await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

        public async Task<User?> FindUserByUserNameAsync(string userName) => await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
        public async Task<User?> FindUserByIDAsync(string userId) => await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

        public async Task<bool> IsUserExistAsync(string userID) => await _userManager.Users.AsNoTracking().AnyAsync(user => user.Id == userID).ConfigureAwait(false);
        #endregion

        #region UserProfileImage
        public async Task<List<ProfileImage>> GetUserProfileImageListAsync() =>
          await _context
          .UserProfileImages
          .AsNoTracking()
          .OrderByDescending(userProfileImg => userProfileImg.Id)
          .ToListAsync()
          .ConfigureAwait(false);

        public async Task<ProfileImage?> GetProfileImageAsync(Guid userProfileImageID) =>
       await _context
      .UserProfileImages.AsNoTracking()
      .SingleOrDefaultAsync(userProfileImg => userProfileImg.Id == userProfileImageID)
      .ConfigureAwait(false);

        public async Task AddProfileImageAsync(ProfileImage userProfileImage)
        {
            await _context.UserProfileImages.AddAsync(userProfileImage).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateProfileImageAsync(ProfileImage profileImage)
        {
            _context.UserProfileImages.Update(profileImage);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteProfileImageAsync(ProfileImage userProfileImage)
        {
            if (userProfileImage == null || userProfileImage.ProfileImageName == null)
                return;
            var environmentPath = _imageService.GetLocalImagesFullPath("auth");
            var profileImagePath = Path.Combine(environmentPath, userProfileImage.ProfileImageName);
            var filesDir = Directory.GetFiles(environmentPath);
            foreach (var filePath in filesDir)
            {
                if(filePath == profileImagePath)
                    File.Delete(filePath);
            }
           _context.UserProfileImages.Remove(userProfileImage);
           await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion

        #region Asset
        public async Task<IEnumerable<Asset>> GetAssetListDescendingAsync() =>
            await _context
                .Assets
                .AsNoTracking()
                .Include(prop => prop.Persons)
                .Include(assetImg => assetImg.AssetImages)
                .OrderByDescending(prop => prop.OrderID)
                .ToListAsync().ConfigureAwait(false);
        public async Task<IEnumerable<Asset>> GetAssetListAscendingAsync() =>
            await _context
                .Assets
                .AsNoTracking()
                .Include(prop => prop.Persons)
                .Include(assetImg => assetImg.AssetImages)
                .OrderBy(prop => prop.OrderID)
                .ToListAsync().ConfigureAwait(false);
        public async Task<IEnumerable<Asset>> GetAssetListDateModifiedAsync() =>
            await _context
                .Assets
                .AsNoTracking()
                .Include(prop => prop.Persons)
                .Include(assetImg => assetImg.AssetImages)
                .OrderBy(prop => prop.Date)
                .ToListAsync().ConfigureAwait(false);
        public async Task<Asset?> GetAssetAsync(Guid assetID) =>
            await _context
                .Assets.AsNoTracking()
                .Include(prop => prop.Persons)
                .Include(assetImg => assetImg.AssetImages)
                .SingleOrDefaultAsync(prop => prop.Id == assetID)
                .ConfigureAwait(false);
        public async Task<Asset?> AddAssetAsync(Asset newAsset)
        {
            await _context.Assets.AddAsync(newAsset).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return newAsset;
        }
        public async Task UpdateAssetAsync(Asset asset)
        {
            _context.Assets.Update(asset);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task DeleteAssetAsync(Asset asset)
        {
            if (asset == null || asset.AssetImages == null)
                return;

            var environmentPath = _imageService.GetLocalImagesFullPath("asset");
            foreach (var assetImg in asset.AssetImages)
            {
                File.Delete(Path.Combine(environmentPath, assetImg.FileName));
            }
            _context.Assets.Remove(asset);
           await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task DeleteAllAssetsAsync()
        {
            var environmentPath = _imageService.GetLocalImagesFullPath("asset");
            if (Directory.Exists(environmentPath))
            {
                var filesDir = Directory.GetFiles(environmentPath);
                foreach (var file in filesDir)
                {
                    File.Delete(file);
                }
            }
            await _context.Assets.ExecuteDeleteAsync().ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<Asset?> FindAssetByPlatesNumberAsync(string platesNumber)
        {
            var assetList = await GetAssetListDescendingAsync().ConfigureAwait(false);
            return assetList.FirstOrDefault(asset => asset.PlatesNumber == platesNumber);
        }
        public async Task<bool> IsAssetExist(string plateNumber) =>
           await _context.Assets.AsNoTracking().AnyAsync(prop => prop.PlatesNumber == plateNumber).ConfigureAwait(false);
        #endregion

        #region AssetImage
        public async Task<List<AssetImage>> GetAssetImageListAsync() =>
           await _context
           .AssetImages
           .AsNoTracking()
           .OrderByDescending(assetImg => assetImg.Id)
           .ToListAsync()
           .ConfigureAwait(false);
        public async Task<AssetImage?> GetAssetImageAsync(Guid assetImageID) =>
         await _context
        .AssetImages.AsNoTracking()
        .SingleOrDefaultAsync(assetImg => assetImg.Id == assetImageID)
        .ConfigureAwait(false);
        public async Task AddAssetImageAsync(AssetImage assetImage)
        {
            await _context.AssetImages.AddAsync(assetImage).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task DeleteAssetImage(AssetImage assetImage)
        {
            if (assetImage == null || assetImage.FileName == null)
                return;
            var environmentPath = _imageService.GetLocalImagesFullPath("asset");
            var filesDir = Directory.GetFiles(environmentPath);
            foreach (var filePath in filesDir)
            {
                var assetImgPath = Path.Combine(environmentPath, assetImage.FileName);
                if (assetImgPath == filePath)
                    File.Delete(filePath);
            }
            _context.AssetImages.Remove(assetImage);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion

        #region Person
        public async Task<IEnumerable<Person>> GetPersonsListAsync() =>
            await _context
                .Persons.AsNoTracking()
                .OrderByDescending(per => per.Id)
                .ToListAsync().ConfigureAwait(false);
        public async Task<Person?> GetPersonAsync(Guid id) =>
            await _context.Persons.AsNoTracking().SingleOrDefaultAsync(pers => pers.Id == id).ConfigureAwait(false);

        public async Task<bool> GetPersonByPersonIDAsync(long personID) =>
            await _context.Persons.AsNoTracking().AnyAsync(pers => pers.PersonID == personID).ConfigureAwait(false);

        public async Task<Person> AddPersonAsync(Person newPerson)
        {
            await _context.Persons.AddAsync(newPerson).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return newPerson;
        }

        public async Task<Person> UpdatePersonAsync(Person updatePerson)
        {
            _context.Persons.Update(updatePerson);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return updatePerson;
        }

        public async Task DeletePersonAsync(Person deletePerson)
        {
            _context.Persons.Remove(deletePerson);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAllPersonsAsync()
        {
            _context.Persons.ExecuteDelete();
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<bool> IsPersonExistAsync(long personID) =>
          await _context.Persons.AsNoTracking().AnyAsync(pers => pers.PersonID == personID).ConfigureAwait(false);
        #endregion

        #region Support
        public async Task<IEnumerable<Support>> GetSupportListAsync() =>
           await _context
          .Supports
          .AsNoTracking()
          .Include(support => support.SupportImage)
          .ToListAsync().ConfigureAwait(false);

        public async Task<Support?> GetSupportAsync(Guid supportID) =>
          await _context
              .Supports.AsNoTracking()
              .Include(sups => sups.SupportImage)
              .SingleOrDefaultAsync(sup => sup.Id == supportID)
              .ConfigureAwait(false);

        public async Task<Support> AddSupportAsync(Support newSupport)
        {
            await _context.Supports.AddAsync(newSupport).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return newSupport;
        }

        public async Task UpdateSupportAsync(Support updateSupport)
        {
            _context.Supports.Update(updateSupport);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task DeleteSupportAsync(Support support)
        {
            if (support == null)
                return;

            if (support.SupportImage != null)
            {
                var environmentPath = _imageService.GetLocalImagesFullPath("support");
                var filesDir = Directory.GetFiles(environmentPath);
                var supImagePath = Path.Combine(environmentPath, support.SupportImage.SupportImageFileName);
                foreach (var filePath in filesDir)
                {
                    if (supImagePath == filePath)
                        File.Delete(filePath);
                }
            }
            _context.Supports.Remove(support);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task DeleteAllSupportsAsync()
        {
            var environmentPath = _imageService.GetLocalImagesFullPath("support");
            if (Directory.Exists(environmentPath))
            {
                var filesPath = Directory.GetFiles(environmentPath);
                foreach (var filePath in filesPath)
                {
                    File.Delete(filePath);
                }
            }
            _context.Supports.ExecuteDelete();
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion

        #region SupportImage
        public async Task<List<SupportImage>> GetSupportImageListAsync() =>
          await _context
          .SupportImages
          .ToListAsync()
          .ConfigureAwait(false);

        public async Task<SupportImage?> GetSupportImageAsync(Guid supportImageID) =>
           await _context
          .SupportImages.AsNoTracking()
          .SingleOrDefaultAsync(supImage => supImage.Id == supportImageID)
          .ConfigureAwait(false);

        public async Task AddSupportImageAsync(SupportImage supportImage)
        {
            await _context.SupportImages.AddAsync(supportImage).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateSupportImageAsync(SupportImage supportImage)
        {
            _context.SupportImages.Update(supportImage);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteSupportImageAsync(SupportImage supportImage)
        {
            if (supportImage == null || supportImage.SupportImageFileName == null)
                return;
            var environmentPath = _imageService.GetLocalImagesFullPath("support");
            var supImgPath = Path.Combine(environmentPath, supportImage.SupportImageFileName);
            var filesPath = Directory.GetFiles(environmentPath);
            foreach (var filePath in filesPath)
            {
                if(supImgPath == filePath)
                    File.Delete(filePath);
            }
            _context.SupportImages.Remove(supportImage);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion

    }

}
