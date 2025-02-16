using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

public class RepositoryService( AppDbContext context,
UserManager<UserProfileIdentity> userManager,
SignInManager<UserProfileIdentity> signInManager,
DatabaseService databaseService)
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<UserProfileIdentity> _userManager = userManager;
    private readonly SignInManager<UserProfileIdentity> _signInManager = signInManager;
    private readonly DatabaseService _databaseService = databaseService;

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
        _databaseService.ResetIdentity(nameof(AppDbContext.Properties));
    }
    #endregion

    #region Person
    public async Task<IEnumerable<Person>> GetPersonsList() =>
        await _context
            .Persons.AsNoTracking()
            .OrderByDescending(per => per.Id)
            .ToListAsync();
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

    public async Task<Person> UpdatePerson(Person updatePerson)
    {
        _context.Persons.Update(updatePerson);
        await _context.SaveChangesAsync();
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
        _databaseService.ResetIdentity(nameof(AppDbContext.Persons));
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
                Id = userInUserManager.Id,
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
        _databaseService.ResetIdentity(nameof(_userManager.Users));
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

     public async Task<IdentityResult> ChangePassword(UserProfileIdentity user,string currentPassword,string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
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
