using Microsoft.AspNetCore.Identity;
using RealEstate.Entities.Users;
using RealEstate.Repositories.Users;

namespace RealEstate.Services.Users;

interface IAdminService
{
    Task<IEnumerable<ApplicationUser>> GetUsersListAsync();
    Task DeleteUsersAsync();
    Task<IdentityResult> PromoteAsync(ApplicationUser user);
}

#pragma warning disable CA1515
public class AdminService(AdminRepository repository) : IAdminService
{
    private readonly AdminRepository _repository = repository;

    public async Task<IEnumerable<ApplicationUser>> GetUsersListAsync() => 
        await _repository.GetUsersListAsync().ConfigureAwait(false);

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

    public async Task<IdentityResult> PromoteAsync(ApplicationUser user) => 
        await _repository.PromoteAsync(user).ConfigureAwait(false);

    public async Task<bool> IsAdmin(ApplicationUser user) => 
        await _repository.IsAdmin(user).ConfigureAwait(false);

}
