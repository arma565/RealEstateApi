using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Users;
using RealEstate.Entities.Users.Authentications;

namespace RealEstate.Repositories.Users.Authentications;

interface ITokenRepository
{
    Task<RefreshToken?> GetRefreshTokensAsync(string tokenHash);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser applicationUser);
    Task<IEnumerable<string>> GetRolesAsync(ApplicationUser applicationUser);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByUsernameAsync(string userName);
}

#pragma warning disable CA1515
public class TokenRepository(UserManager<ApplicationUser> userManager,AppDbContext context) : ITokenRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly AppDbContext _context = context;

    public async Task<RefreshToken?> GetRefreshTokensAsync(string tokenHash) =>
         await _context.RefreshTokens
            .Include(refreshToken => refreshToken.Agent)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash)
            .ConfigureAwait(false);

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser applicationUser) =>
         await _userManager.GeneratePasswordResetTokenAsync(applicationUser).ConfigureAwait(false);

    public async Task<IEnumerable<string>> GetRolesAsync(ApplicationUser applicationUser) =>
        await _userManager.GetRolesAsync(applicationUser).ConfigureAwait(false);
   
    public async Task<ApplicationUser?> FindByEmailAsync(string email) =>
     await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

    public async Task<ApplicationUser?> FindByUsernameAsync(string userName) =>
         await _userManager.FindByNameAsync(userName).ConfigureAwait(false);

}
