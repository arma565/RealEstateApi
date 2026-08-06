using Microsoft.AspNetCore.Identity;
using RealEstate.DTOs.Users;
using RealEstate.Entities.Users;
using RealEstate.Repositories.Users;

namespace RealEstate.Services.Users;

interface IUserService
{
    Task<ApplicationUser?> GetByIDAsync(string id);
    Task<ApplicationUser?> GetByUserNameAsync(string userName);
    Task<IdentityResult> RegisterAsync(UserRegisterAccountDTO userRegisterAccountDTO);
    Task<SignInResult> LoginAsync(UserLoginRequestDTO userRequest);
    Task<IdentityResult> DeleteAsync(string id);
    Task<string> GenerateTokenToRecoverUserAsync(string email);
    Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);
    Task<IdentityResult> ChangePasswordAsync(string userName, string currentPassword, string newPassword);
    Task<IdentityResult> EditUserProfileAsync(string id,ApplicationUser applicationUser);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByUserNameAsync(string userName);
    Task<ApplicationUser?> FindByIDAsync(string userId);
    Task<bool> IsEmailConfirmed(ApplicationUser applicationUser);
}

#pragma warning disable CA1515
public class UserService(UserRepository repository) : IUserService
{
    private readonly UserRepository _repository = repository;
    //private readonly ImageRepository _imageRepository = imageRepository;

    public async Task<ApplicationUser?> GetByIDAsync(string id) =>
         await _repository.GetByIDAsync(id).ConfigureAwait(false);

    public async Task<ApplicationUser?> GetByUserNameAsync(string userName) =>
         await _repository.GetByUserNameAsync(userName).ConfigureAwait(false);

    public async Task<IdentityResult> RegisterAsync(UserRegisterAccountDTO userRegisterAccountDTO)
    {
        ArgumentNullException.ThrowIfNull(userRegisterAccountDTO);

        var applicationUser = new ApplicationUser
        {
            UserName = userRegisterAccountDTO.UserName,
            Email = userRegisterAccountDTO.Email,
            AcceptTerms = userRegisterAccountDTO.AcceptTerms
        };

        return await _repository.RegisterAsync(applicationUser, userRegisterAccountDTO.Password).ConfigureAwait(false);
    }

    public async Task<SignInResult> LoginAsync(UserLoginRequestDTO userRequest)
    {
        if (userRequest is null)
            return SignInResult.Failed;

        return await _repository.LoginAsync(userRequest.UserName, userRequest.Password).ConfigureAwait(false);
    }

    public async Task<IdentityResult> DeleteAsync(string id)
    {
        var user = await _repository.GetByIDAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(user);

        //await _imageRepository.DeleteAsync(user.ProfileImage.Id).ConfigureAwait(false);

        return await _repository.DeleteAsync(user).ConfigureAwait(false);
    }

    public async Task<string> GenerateTokenToRecoverUserAsync(string email) {

        var user = await _repository.FindByEmailAsync(email).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(user);

        return await _repository.GenerateTokenToRecoverUserAsync(user).ConfigureAwait(false);
    }
  
    public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword) {

        var user = await _repository.FindByEmailAsync(email).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(user);

       return await _repository.ResetPasswordAsync(user, token, newPassword).ConfigureAwait(false);
    }

    public async Task<IdentityResult> ChangePasswordAsync(string userName, string currentPassword, string newPassword) {

        var user = await _repository.FindByUserNameAsync(userName).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(user);

      return  await _repository.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);

    }

    public async Task<IdentityResult> EditUserProfileAsync(string id , ApplicationUser applicationUser) {

        ArgumentNullException.ThrowIfNull(applicationUser);

        if (id != applicationUser.Id)
            return IdentityResult.Failed(new IdentityError { Description = "userId and application user id are not match!" });

        var user = await _repository.FindByIDAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrEmpty(applicationUser.UserName) || string.IsNullOrEmpty(applicationUser.Email))
        {
            user.UserName = user.UserName;
            user.Email = user.Email;
        }
        else
        {
            user.UserName = applicationUser.UserName;
            user.Email = applicationUser.Email;
        }

        user.Id = applicationUser.Id;
        user.FirstName = applicationUser.FirstName;
        user.LastName = applicationUser.LastName;
        user.PhoneNumber = applicationUser.PhoneNumber;
        user.AcceptTerms = user.AcceptTerms;
        user.ImageId = applicationUser.ImageId;

        return await _repository.EditUserProfileAsync(user).ConfigureAwait(false);
    } 

    public async Task<ApplicationUser?> FindByEmailAsync(string email) => await _repository.FindByEmailAsync(email).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByUserNameAsync(string userName) => await _repository.FindByUserNameAsync(userName).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByIDAsync(string userId) => await _repository.FindByIDAsync(userId).ConfigureAwait(false);

    public async Task<bool> IsEmailConfirmed(ApplicationUser applicationUser) => await _repository.IsEmailConfirmedAsync(applicationUser).ConfigureAwait(false);
}
