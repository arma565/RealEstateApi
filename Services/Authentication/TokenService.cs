using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Services.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstate.Services.Authentication;

#pragma warning disable CA1515
interface ITokenService
{
    Task<string> CreateAccessTokenAsync(ApplicationUser user);
}
public class TokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> options) : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtOptions _jwt = options.Value;

    public async Task<string> CreateAccessTokenAsync(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

        var claims = new List<Claim>{
            new(JwtRegisteredClaimNames.Sub , user.Id),
            new(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier , user.Id),
            new(ClaimTypes.Name , user.UserName!),
            new(ClaimTypes.Email , user.Email!),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}
