using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Data;
using RealEstate.Entities.Users;
using RealEstate.Entities.Users.Authentications;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RealEstate.Services.Users.Authentication;

#pragma warning disable CA1515
internal interface ITokenService
{
    Task<TokenResponse> CreateAccessTokenAsync(string userName);
}
public class TokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> options , AppDbContext context) : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtOptions _jwt = options.Value;
    private readonly AppDbContext _context = context;

    public async Task<TokenResponse> CreateAccessTokenAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);
        
        if(string.IsNullOrEmpty(user.UserName))
            throw new InvalidOperationException("Create token failed: Username is required!");

        if (string.IsNullOrEmpty(user.Email))
            throw new InvalidOperationException("Create token failed: Email is required!");

        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

        var claims = new List<Claim>{
            new(JwtRegisteredClaimNames.Sub , user.Id),
            new(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier , user.Id),
            new(ClaimTypes.Name , user.UserName),
            new(ClaimTypes.Email , user.Email),
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
            expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = HashRefreshToken(refreshToken),
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await _context.RefreshTokens.AddAsync(refreshTokenEntity).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return new TokenResponse(AccessToken: accessToken , RefreshToken: refreshToken);

    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    public static string HashRefreshToken(string token)
    {
        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToBase64String(hash);
    }
}



public sealed record TokenResponse(
string AccessToken,
string RefreshToken);
