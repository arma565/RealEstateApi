using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Data;
using RealEstate.Models.Authentication;
using RealEstate.Models.Authentication.Users;
using RealEstate.Models.Estate;
using RealEstate.Models.Estate.Assets;
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
        public async Task<User?> GetUserByID(string userID) =>
             await _userManager
                 .Users.AsNoTracking()
                 .Include(user => user.ProfileImage)
                 .SingleOrDefaultAsync(user => user.Id == userID)
                 .ConfigureAwait(false);

        /// <summary>
        /// This function return a user using userName
        /// </summary>
        /// <returns> User associated to userName </returns>
        public async Task<User?> GetUserByUserName(string userName) =>
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
        public async Task<IdentityResult> RegisterUser(Register userRegister)
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
        public async Task DeleteAllUsers()
        {
            var users = await _userManager.Users.ToListAsync().ConfigureAwait(false);
           foreach (var user in users)
            {
                await _userManager.DeleteAsync(user).ConfigureAwait(false);
            }
            if (Directory.Exists(Path.Combine("wwwroot/images/auth"))) {
                var files = Directory.GetFiles(Path.Combine("wwwroot/images/auth"));
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
        public async Task<IdentityResult> DeleteUser(User user)
        {
            var environmentPath = _imageService.GetLocalImagesFullPath("auth");

            var filePath = Path.Combine(environmentPath, user?.ProfileImage?.ProfileImageName ?? "");

            if (File.Exists(filePath))
                File.Delete(filePath);

            return await _userManager.DeleteAsync(user!).ConfigureAwait(false);
        }

        /// <summary>
        /// Login user using username and password
        /// </summary>
        /// <param name="model">
        /// Login model containing username and password
        /// </param>
        /// <returns></returns>
        public async Task<SignInResult> LoginUser(Login model)
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
        public async Task<string> GenerateTokenToRecoverUser(User user)
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
        public async Task<IdentityResult> ResetPassword(
            User user,
            string token,
            string newPassword
        )
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false);
        }

        public async Task<IdentityResult> ChangePassword(User user, string currentPassword, string newPassword)
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
        public async Task<IdentityResult> EditUserProfile(User updateUser) => await _userManager.UpdateAsync(updateUser).ConfigureAwait(false);

        public async Task<User?> FindUserByEmail(string email) => await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

        public async Task<User?> FindUserByUserName(string userName) => await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
        public async Task<User?> FindUserByID(string userId) => await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

        public async Task<bool> isUserExist(string userID) => await _userManager.Users.AsNoTracking().AnyAsync(user => user.Id == userID).ConfigureAwait(false);
        #endregion

        #region UserProfileImage
        public async Task<List<ProfileImage>> GetUserProfileImageList() =>
          await _context
          .UserProfileImages
          .AsNoTracking()
          .OrderByDescending(userProfileImg => userProfileImg.Id)
          .ToListAsync()
          .ConfigureAwait(false);

        public async Task<ProfileImage?> GetProfileImage(Guid userProfileImageID) =>
       await _context
      .UserProfileImages.AsNoTracking()
      .SingleOrDefaultAsync(userProfileImg => userProfileImg.Id == userProfileImageID)
      .ConfigureAwait(false);

        public async Task AddProfileImage(ProfileImage userProfileImage)
        {
            await _context.UserProfileImages.AddAsync(userProfileImage).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateProfileImage(ProfileImage profileImage)
        {
            _context.UserProfileImages.Update(profileImage);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void DeleteProfileImage(ProfileImage userProfileImage)
        {
            if (userProfileImage == null)
                return;

            if (userProfileImage.ProfileImageName != null)
            {
                var files = Directory.GetFiles(Path.Combine("wwwroot/images/auth"));
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
            _context.UserProfileImages.Remove(userProfileImage);
            _context.SaveChanges();
        }
        #endregion

        #region Asset
        public async Task<IEnumerable<Asset>> GetAssetListDescending() =>
            await _context
                .Assets
                .AsNoTracking()
                .Include(prop => prop.Persons)
                .Include(assetImg => assetImg.AssetImages)
                .OrderByDescending(prop => prop.OrderID)
                .ToListAsync().ConfigureAwait(false);
        public async Task<IEnumerable<Asset>> GetAssetListAscending() =>
            await _context
                .Assets
                .AsNoTracking()
                .Include(prop => prop.Persons)
                .Include(assetImg => assetImg.AssetImages)
                .ToListAsync().ConfigureAwait(false);
        public async Task<IEnumerable<Asset>> GetAssetListDateModified() =>
            await _context
                .Assets
                .AsNoTracking()
                .Include(prop => prop.Persons)
                .Include(assetImg => assetImg.AssetImages)
                .OrderBy(prop => prop.Date)
                .ToListAsync().ConfigureAwait(false);
        public async Task<Asset?> GetAsset(Guid assetID) =>
            await _context
                .Assets.AsNoTracking()
                .Include(prop => prop.Persons)
                .Include(assetImg => assetImg.AssetImages)
                .SingleOrDefaultAsync(prop => prop.Id == assetID)
                .ConfigureAwait(false);
        public async Task<Asset?> AddAsset(Asset newAsset)
        {
            await _context.Assets.AddAsync(newAsset).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return newAsset;
        }
        public async Task UpdateAsset(Asset updateAsset)
        {
            _context.Assets.Update(updateAsset);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public void DeleteAsset(Asset deleteAsset)
        {
            if (deleteAsset == null)
                return;
               
            if (deleteAsset.AssetImages != null) {
                var files = Directory.GetFiles(Path.Combine("wwwroot/images/Asset"));
                foreach (var file in files)
                {
                    if (deleteAsset.AssetImages.Any(assetImg => assetImg.FileName == file))
                    {
                        File.Delete(file);
                    }
                }
            }
            _context.Assets.Remove(deleteAsset);
            _context.SaveChanges();
        }
        public void DeleteAllAssets()
        {
            _context.Assets.ExecuteDelete();
            _context.SaveChanges();
            if (Directory.Exists(Path.Combine("wwwroot/images/Asset")))
            {
                var files = Directory.GetFiles(Path.Combine("wwwroot/images/Asset"));
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }
        public async Task<Asset?> FindAssetByPlatesNumber(string platesNumber)
        {
            var assetList = await GetAssetListDescending().ConfigureAwait(false);
            return assetList.FirstOrDefault(asset => asset.PlatesNumber == platesNumber);
        }
        public async Task<bool> IsAssetExist(string plateNumber) =>
           await _context.Assets.AsNoTracking().AnyAsync(prop => prop.PlatesNumber == plateNumber).ConfigureAwait(false);
        #endregion

        #region AssetImage
        public async Task<List<AssetImage>> GetAssetImageList() =>
           await _context
           .AssetImages
           .AsNoTracking()
           .OrderByDescending(assetImg => assetImg.Id)
           .ToListAsync()
           .ConfigureAwait(false);
        public async Task<AssetImage?> GetAssetImage(Guid assetImageID) =>
         await _context
        .AssetImages.AsNoTracking()
        .SingleOrDefaultAsync(assetImg => assetImg.Id == assetImageID)
        .ConfigureAwait(false);
        public async Task AddAssetImage(AssetImage assetImage)
        {
            await _context.AssetImages.AddAsync(assetImage).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public void DeleteAssetImage(AssetImage deleteAssetImage)
        {
            if (deleteAssetImage == null)
                return;

            if (deleteAssetImage.FileName != null)
            {
                var files = Directory.GetFiles(Path.Combine("wwwroot/images/Asset"));
                foreach (var file in files)
                {
                    if (deleteAssetImage.FileName == file)
                    {
                        File.Delete(file);
                    }
                }
            }
            _context.AssetImages.Remove(deleteAssetImage);
            _context.SaveChanges();
        }
        #endregion

        #region Person
        public async Task<IEnumerable<Person>> GetPersonsList() =>
            await _context
                .Persons.AsNoTracking()
                .OrderByDescending(per => per.Id)
                .ToListAsync().ConfigureAwait(false);
        public async Task<Person?> GetPerson(Guid id) =>
            await _context.Persons.AsNoTracking().SingleOrDefaultAsync(pers => pers.Id == id).ConfigureAwait(false);

        public async Task<bool> GetPersonByPersonID(long personID) =>
            await _context.Persons.AsNoTracking().AnyAsync(pers => pers.PersonID == personID).ConfigureAwait(false);

        public async Task<Person> AddPerson(Person newPerson)
        {
            await _context.Persons.AddAsync(newPerson).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return newPerson;
        }

        public async Task<Person> UpdatePerson(Person updatePerson)
        {
            _context.Persons.Update(updatePerson);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return updatePerson;
        }

        public void DeletePerson(Person deletePerson)
        {
            _context.Persons.Remove(deletePerson);
            _context.SaveChanges();
        }

        public void DeleteAllPersons()
        {
            _context.Persons.ExecuteDelete();
            _context.SaveChanges();
        }

        private IEnumerable<Person> GetPersonList() =>
            [.. _context.Persons.AsNoTracking().OrderByDescending(pers => pers.Id)];
        #endregion

    }

}
