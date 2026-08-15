using Microsoft.AspNetCore.Identity;
using RealEstate.DTOs.Users;
using RealEstate.Entities.Users;
using RealEstate.Repositories.Users;

namespace RealEstate.Services.Users;

interface IAdminService
{
    Task<IEnumerable<ApplicationUser>> GetUsersListAsync();
    Task<ApplicationUser> GetAsync(string id);
    Task<ApplicationUser> GetByUserNameAsync(string userName);
    Task<IdentityResult> RegisterAsync(RegisterAccountDTO userRegisterAccountDTO);
    Task DeleteAsync(string id);
    Task DeleteUsersAsync();
    Task<IdentityResult> PromoteAsync(string userName);
}

#pragma warning disable CA1515
public class AdminService(AdminRepository repository) : IAdminService
{
    private readonly AdminRepository _repository = repository;

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

        var result = await _repository.RegisterAsync(new ApplicationUser
        {
            UserName = userRegisterAccountDTO.UserName,
            Email = userRegisterAccountDTO.Email,
            AcceptTerms = userRegisterAccountDTO.AcceptTerms
        }, userRegisterAccountDTO.Password).ConfigureAwait(false);

        if (result.Succeeded)
        {
            await _repository.AssignAsync(userRegisterAccountDTO.UserName).ConfigureAwait(false);
            return result;
        }
        else
            return IdentityResult.Failed();
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        await _repository.DeleteUserAsync(user).ConfigureAwait(false);
    }

    public async Task DeleteUsersAsync()
    {
        var users = await GetUsersListAsync().ConfigureAwait(false);
        foreach (var user in users)
        {
            if (await _repository.IsAdmin(user).ConfigureAwait(false))
                continue;
            await _repository.DeleteUserAsync(user).ConfigureAwait(false);
        }
    }

    public async Task<IdentityResult> PromoteAsync(string userName)
    {
        var registeredUser = await _repository.GetByUserNameAsync(userName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(registeredUser);

        return await _repository.PromoteAsync(registeredUser).ConfigureAwait(false);
    }

    public async Task<bool> IsAdmin(ApplicationUser user) =>
        await _repository.IsAdmin(user).ConfigureAwait(false);

}
