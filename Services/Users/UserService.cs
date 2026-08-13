using Microsoft.AspNetCore.Identity;
using RealEstate.DTOs.Users;
using RealEstate.Entities.Users;
using RealEstate.Repositories.Users;

namespace RealEstate.Services.Users;

interface IUserService
{
    Task<ApplicationUser> GetAsync(string id);
    Task<ApplicationUser> GetByUserNameAsync(string userName);
    Task<IdentityResult> RegisterAsync(RegisterAccountDTO userRegisterAccountDTO);
    Task<SignInResult> LoginAsync(LoginRequestDTO userRequest);
    Task<IdentityResult> DeleteAsync(string id);
    Task<string> GenerateTokenToRecoverUserAsync(string email);
    Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
    Task<IdentityResult> ChangePasswordAsync(string userName, string currentPassword, string newPassword);
    Task<IdentityResult> EditUserProfileAsync(string id, EditProfileDTO editProfileDTO);
    Task<bool> IsEmailConfirmed(ApplicationUser applicationUser);
}

#pragma warning disable CA1515
public class UserService(UserRepository repository) : IUserService
{
    private readonly UserRepository _repository = repository;

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

        return await _repository.RegisterAsync(new ApplicationUser
        {
            UserName = userRegisterAccountDTO.UserName,
            Email = userRegisterAccountDTO.Email,
            AcceptTerms = userRegisterAccountDTO.AcceptTerms
        }, userRegisterAccountDTO.Password).ConfigureAwait(false);
    }

    public async Task<SignInResult> LoginAsync(LoginRequestDTO userRequest) =>
         userRequest is null ? SignInResult.Failed : await _repository.LoginAsync(userRequest.UserName, userRequest.Password).ConfigureAwait(false);

    public async Task<IdentityResult> DeleteAsync(string id)
    {
        var user = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        return await _repository.DeleteAsync(user).ConfigureAwait(false);
    }

    public async Task<string> GenerateTokenToRecoverUserAsync(string email)
    {

        var user = await _repository.FindByEmailAsync(email).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        return await _repository.GenerateTokenToRecoverUserAsync(user).ConfigureAwait(false);
    }

    public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
    {

        var user = await _repository.FindByEmailAsync(email).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        return await _repository.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false);
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userName, string currentPassword, string newPassword)
    {

        var user = await _repository.GetByUserNameAsync(userName).ConfigureAwait(false);
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

    public async Task<bool> IsEmailConfirmed(ApplicationUser applicationUser) => await _repository.IsEmailConfirmedAsync(applicationUser).ConfigureAwait(false);
}
