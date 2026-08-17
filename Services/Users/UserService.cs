using Microsoft.AspNetCore.Identity;
using RealEstate.DTOs.Users;
using RealEstate.Entities.Users;
using RealEstate.Repositories.Users;
using RealEstate.Services.Users.Authentication;

namespace RealEstate.Services.Users;

interface IUserService
{
    Task<IEnumerable<ApplicationUser>> GetUsersListAsync();
    Task<ApplicationUser> GetAsync(string id);
    Task<ApplicationUser> GetByUserNameAsync(string userName);
    Task<IdentityResult> RegisterAsync(RegisterAccountDTO userRegisterAccountDTO);
    Task<string> LoginAsync(LoginRequestDTO userRequest);
    Task<string> GenerateTokenAsync(string email);
    Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
    Task<IdentityResult> ChangePasswordAsync(string userName, string currentPassword, string newPassword);
    Task<IdentityResult> EditUserProfileAsync(string id, EditProfileDTO editProfileDTO);
    Task DeleteAsync(string id);
    Task DeleteUsersAsync();
    Task<IdentityResult> PromoteAsync(string userName);
    Task<IdentityResult> DemoteAsync(string userName);
    Task<bool> IsAdmin(ApplicationUser user);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<bool> IsEmailConfirmed(ApplicationUser applicationUser);
}

#pragma warning disable CA1515
public class UserService(UserRepository repository, TokenService tokenService) : IUserService
{
    private readonly UserRepository _repository = repository;

    private readonly TokenService _tokenService = tokenService;

    public async Task<IEnumerable<ApplicationUser>> GetUsersListAsync() =>
         await _repository.GetUsersListAsync().ConfigureAwait(false);

    public async Task<ApplicationUser> GetAsync(string id)
    {
        var applicationUser = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(applicationUser);
        return applicationUser;
    }

    public async Task<ApplicationUser> GetByUserNameAsync(string userName)
    {
        var applicationUser = await _repository.GetByUserNameAsync(userName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(applicationUser);
        return applicationUser;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterAccountDTO userRegisterAccountDTO)
    {
        ArgumentNullException.ThrowIfNull(userRegisterAccountDTO);

        var allUsers = await GetUsersListAsync().ConfigureAwait(false);

        if (allUsers.Any(u => u.UserName == userRegisterAccountDTO.UserName) || allUsers.Any(u => u.Email == userRegisterAccountDTO.Email))
            throw new InvalidOperationException("Username or email is already taken!");

        var result = await _repository.RegisterAsync(new ApplicationUser
        {
            UserName = userRegisterAccountDTO.UserName,
            Email = userRegisterAccountDTO.Email,
            AcceptTerms = userRegisterAccountDTO.AcceptTerms
        }, userRegisterAccountDTO.Password).ConfigureAwait(false);

        if (result.Succeeded)
        {
            await AssignAsync(userRegisterAccountDTO.UserName).ConfigureAwait(false);
            return result;
        }
        else
            return IdentityResult.Failed();
    }

    public async Task<TokenResponse> LoginAsync(LoginRequestDTO userRequest)
    {

        ArgumentNullException.ThrowIfNull(userRequest);
        var result = await _repository.LoginAsync(userRequest.UserName, userRequest.Password).ConfigureAwait(false);

        if (!result.Succeeded)
            throw new InvalidOperationException("Sign in failed!");

        return await _tokenService.CreateAccessTokenAsync(userRequest.UserName).ConfigureAwait(false);
    }

    public async Task<string> GenerateTokenAsync(string email)
    {

        var user = await _repository.FindByEmailAsync(email).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        return await _repository.GenerateTokenAsync(user).ConfigureAwait(false);
    }

    public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
    {

        var user = await _repository.FindByEmailAsync(email).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        return await _repository.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false);
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userName, string currentPassword, string newPassword) {
        var user = await _repository.FindByUsernameAsync(userName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

       return await _repository.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);
    }

    public async Task<IdentityResult> EditUserProfileAsync(string id, EditProfileDTO editProfileDTO)
    {

        var user = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        ArgumentNullException.ThrowIfNull(editProfileDTO);

        user.UserName = string.IsNullOrEmpty(editProfileDTO.UserName) ? user.UserName : editProfileDTO.UserName;
        user.Email = string.IsNullOrEmpty(editProfileDTO.Email) ? user.Email : editProfileDTO.Email;
        user.FirstName = string.IsNullOrEmpty(editProfileDTO.FirstName) ? user.FirstName : editProfileDTO.FirstName;
        user.LastName = string.IsNullOrEmpty(editProfileDTO.LastName) ? user.LastName : editProfileDTO.LastName;
        user.PhoneNumber = string.IsNullOrEmpty(editProfileDTO.PhoneNumber) ? user.PhoneNumber : editProfileDTO.PhoneNumber;

        return await _repository.EditUserProfileAsync(user).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        if (await _repository.IsAdmin(user).ConfigureAwait(false) || await _repository.IsManager(user).ConfigureAwait(false))
            throw new InvalidOperationException("Admin or manager can not be deleted!");
        else
            await _repository.DeleteAsync(user).ConfigureAwait(false);
    }

    public async Task DeleteUsersAsync()
    {
        var users = await GetUsersListAsync().ConfigureAwait(false);
        foreach (var user in users)
        {
            if (await _repository.IsAdmin(user).ConfigureAwait(false) || await _repository.IsManager(user).ConfigureAwait(false))
                continue;
            await _repository.DeleteAsync(user).ConfigureAwait(false);
        }
    }

    public async Task<IdentityResult> PromoteAsync(string userName)
    {
        var registeredUser = await _repository.FindByUsernameAsync(userName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(registeredUser);

        return await _repository.PromoteAsync(registeredUser).ConfigureAwait(false);
    }

    public async Task<IdentityResult> DemoteAsync(string userName)
    {
        var registeredUser = await _repository.FindByUsernameAsync(userName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(registeredUser);

        return await _repository.DemoteAsync(registeredUser).ConfigureAwait(false);
    }

    public async Task<bool> IsAdmin(ApplicationUser user) =>
        await _repository.IsAdmin(user).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByEmailAsync(string email) =>
         await _repository.FindByEmailAsync(email).ConfigureAwait(false);

    public async Task<bool> IsEmailConfirmed(ApplicationUser applicationUser) => await _repository.IsEmailConfirmedAsync(applicationUser).ConfigureAwait(false);

    private async Task AssignAsync(string userName) {
        var user = await _repository.FindByUsernameAsync(userName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        await _repository.AssignAsync(user).ConfigureAwait(false);
    }
               
    
}
