using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Models.Authentication;
using RealEstate.Models.Estate;
using RealEstate.Models.Estate.Assets;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace RealEstate.Services
{
    public sealed class RepositoryService(AppDbContext context,
UserManager<UserProfileIdentity> userManager,
SignInManager<UserProfileIdentity> signInManager)
    {
        private readonly AppDbContext _context = context;
        private readonly UserManager<UserProfileIdentity> _userManager = userManager;
        private readonly SignInManager<UserProfileIdentity> _signInManager = signInManager;

        #region Authentication

        /// <summary>
        /// This function return all registered users
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync().ConfigureAwait(false);
            var usersList = new List<User>();
            foreach (var userInUserManager in users)
            {
                var user = new User
                {
                    Id = userInUserManager.Id,
                    ProfileImagePath = userInUserManager.ProfileImageName,
                    FirstName = userInUserManager.FirstName!,
                    LastName = userInUserManager.LastName!,
                    AcceptTerms = userInUserManager.AcceptTerms,
                    UserName = userInUserManager.UserName ?? "",
                    Email = userInUserManager.Email ?? "",
                    PhoneNumber = userInUserManager.PhoneNumber ?? "",
                };
                usersList.Add(user);
            }
            return [.. usersList];
        }

        /// <summary>
        /// This function register a user in database
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RegisterUser(Register model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            else
                return await _userManager.CreateAsync(
                       new UserProfileIdentity
                       {
                           UserName = model.UserName,
                           Email = model.Email,
                           AcceptTerms = model.AcceptTerms,
                       },
                       model.Password
                   ).ConfigureAwait(false);
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
            var files = Directory.GetFiles(Path.Combine("wwwroot/images/auth"));
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// This function Delete a user from identity store
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IdentityResult> DeleteUser(UserProfileIdentity user)
        {
            if (user is null)
            {
                return IdentityResult.Failed();
            }
            if (user.ProfileImageName != null)
            {
                Uri uri = new(user.ProfileImageName.ToString());
                var profileImageName = Path.GetFileName(uri.AbsolutePath);
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var filePath = Path.Combine(webRootPath, "images", profileImageName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return await _userManager.DeleteAsync(user).ConfigureAwait(false);
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
        public async Task<string> GenerateTokenToRecoverUser(UserProfileIdentity user)
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
            UserProfileIdentity user,
            string token,
            string newPassword
        )
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false);
        }

        public async Task<IdentityResult> ChangePassword(UserProfileIdentity user, string currentPassword, string newPassword)
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
        public async Task<IdentityResult> EditUserProfile(UserProfileIdentity user)
        {
            return await _userManager.UpdateAsync(user).ConfigureAwait(false);
        }

        public async Task<UserProfileIdentity?> FindUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
        }

        public async Task<UserProfileIdentity?> FindUserByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
        }
        #endregion

        #region Asset
        public async Task<IEnumerable<Asset>> GetAssetList() =>
            await _context
                .Assets
                .AsNoTracking()
                .Include(prop => prop.Persons)
                .Include(assetImg => assetImg.AssetImages)
                .OrderByDescending(prop => prop.Id)
                .ToListAsync().ConfigureAwait(false);

        public async Task<Asset?> GetAsset(Guid assetID) =>
            await _context
                .Assets.AsNoTracking()
                .SingleOrDefaultAsync(prop => prop.Id == assetID)
                .ConfigureAwait(false);

        public async Task<List<AssetImage>> GetAssetImagesList() => 
            await _context
            .AssetImages
            .AsNoTracking()
            .OrderByDescending(assetImg => assetImg.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        public async Task<Asset?> AddAsset(Asset newAsset)
        {
            await _context.Assets.AddAsync(newAsset).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return newAsset;
        }

        public async Task AddAssetImage(AssetImage assetImage)
        {
            await _context.AssetImages.AddAsync(assetImage).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task UpdateAsset(Asset updateAsset)
        {
            _context.Assets.Update(updateAsset);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void DeleteAsset(Asset deleteAsset)
        {
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
            var assetList = await GetAssetList().ConfigureAwait(false);
            return assetList.FirstOrDefault(asset => asset.PlatesNumber == platesNumber);
        }
        public async Task<bool> IsAssetExist(string plateNumber) =>
           await _context.Assets.AsNoTracking().AnyAsync(prop => prop.PlatesNumber == plateNumber).ConfigureAwait(false);
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
