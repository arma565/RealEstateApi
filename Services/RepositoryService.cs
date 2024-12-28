using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

public class RepositoryService
{
    private readonly AppDbContext _context;
    private readonly UserManager<UserProfileIdentity> _userManager;
    private readonly SignInManager<UserProfileIdentity> _signInManager;

    public RepositoryService(
        AppDbContext context,
        UserManager<UserProfileIdentity> userManager,
        SignInManager<UserProfileIdentity> signInManager
    )
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    #region Property
    public async Task<IEnumerable<Property>> GetPropertyList() =>
        await _context
            .Properties.AsNoTracking()
            .Include(prop => prop.Persons)
            .OrderByDescending(prop => prop.Id)
            .ToListAsync();

    public async Task<Property?> GetProperty(int propertyID) =>
        await _context
            .Properties.AsNoTracking()
            .SingleOrDefaultAsync(prop => prop.Id == propertyID);

    public async Task<bool> GetPropertyByPlateNumber(string plateNumber) =>
        await _context.Properties.AsNoTracking().AnyAsync(prop => prop.PlatesNumber == plateNumber);

    public async Task<Property?> AddProperty(Property newProperty)
    {
        await _context.Properties.AddAsync(newProperty);
        await _context.SaveChangesAsync();
        return newProperty;
    }

    public async Task UpdateProperty(Property updateProperty)
    {
        _context.Properties.Update(updateProperty);
        await _context.SaveChangesAsync();
    }

    public void DeleteProperty(Property deleteProperty)
    {
        _context.Properties.Remove(deleteProperty);
        _context.SaveChanges();
    }

    public void DeleteAllProperties()
    {
        _context.Properties.ExecuteDelete();
        _context.SaveChanges();
    }
    #endregion

    #region Person
    public async Task<Person?> GetPerson(int id) =>
        await _context.Persons.AsNoTracking().SingleOrDefaultAsync(pers => pers.Id == id);

    public async Task<bool> GetPersonByPersonID(long personID) =>
        await _context.Persons.AsNoTracking().AnyAsync(pers => pers.PersonID == personID);

    public async Task<Person> AddPerson(Person newPerson)
    {
        await _context.Persons.AddAsync(newPerson);
        await _context.SaveChangesAsync();
        return newPerson;
    }

    public void UpdatePerson(Person updatePerson)
    {
        _context.Persons.Update(updatePerson);
        _context.SaveChanges();
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

    #region Authentication

    /// <summary>
    /// This function return all registered users
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<User>> GetAllUsers()
    {
        var users = await _userManager.Users.AsNoTracking().ToListAsync();
        var usersList = new List<User>();
        foreach (var userInUserManager in users)
        {
            var user = new User
            {
                ProfileImagePath = userInUserManager.ProfileImageUrl!,
                FirstName = userInUserManager.FirstName!,
                LastName = userInUserManager.LastName!,
                AcceptTerms = userInUserManager.AcceptTerms,
                UserName = userInUserManager.UserName ?? "",
                Email = userInUserManager.Email ?? "",
                PhoneNumber = userInUserManager.PhoneNumber ?? "",
            };
            usersList.Add(user);
        }
        return usersList.ToArray();
    }

    /// <summary>
    /// This function register a user in database
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<IdentityResult> RegisterUser(Register model)
    {
        return await _userManager.CreateAsync(
            new UserProfileIdentity
            {
                UserName = model.UserName,
                Email = model.Email,
                AcceptTerms = model.AcceptTerms,
            },
            model.Password
        );
    }

    /// <summary>
    /// This function delete all users
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        foreach (var user in users)
        {
            await _userManager.DeleteAsync(user);
        }
        var files = Directory.GetFiles(Path.Combine("wwwroot/images"));
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
        if (!string.IsNullOrEmpty(user.ProfileImageUrl))
        {
            Uri uri = new Uri(user.ProfileImageUrl);
            var profileImageName = Path.GetFileName(uri.AbsolutePath);
            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(webRootPath, "images", profileImageName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        return await _userManager.DeleteAsync(user);
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
        return await _signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            false,
            false
        );
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
        return await _userManager.GeneratePasswordResetTokenAsync(user);
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
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    /// <summary>
    /// Use this function to upload profile image to server
    /// </summary>
    /// <param name="image">
    /// image to upload
    /// </param>
    /// <returns></returns>
    public async Task<string> UploadProfileImage(IFormFile image)
    {
        var imageDir = Path.Combine("wwwroot", "images");
        var filePath = Path.Combine(
            imageDir,
            Guid.CreateVersion7().ToString() + Path.GetExtension(image.FileName)
        );
        if (!Directory.Exists(imageDir))
        {
            Directory.CreateDirectory(imageDir);
        }

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }
        return Path.GetFileName(filePath);
    }

    /// <summary>
    /// Use this to download image from server
    /// </summary>
    /// <param name="filePath">
    /// file path of image file
    /// </param>
    /// <returns></returns>
    /// <exception cref="IOException"></exception>
    public FileStream ReadProfileImage(string filePath)
    {
        try
        {
            return new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                useAsync: true
            );
        }
        catch (IOException ex)
        {
            throw new IOException("Error reading the file. Error =" + ex.Message);
        }
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
        return await _userManager.UpdateAsync(user);
    }

    public async Task<UserProfileIdentity?> FindUserByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<UserProfileIdentity?> FindUserByUserName(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }
    #endregion
}
