using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstate.DTOs.Users;
using RealEstate.Entities.Users;
using RealEstate.Entities.Users.Authentications;
using RealEstate.Repositories.Users.Authentications;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RealEstate.Services.Users.Authentications;

#pragma warning disable CA1515
internal interface ITokenService
{
    Task<TokenResponse> CreateTokensAsync(ApplicationUser applicationUser);
    Task<TokenResponse> RefreshToken(RefreshTokenRequest request);
    Task<RefreshToken> GetRefreshTokensAsync(RefreshTokenRequest request);
    Task<string> GeneratePasswordResetTokenAsync(string email);
}
public class TokenService(TokenRepository repository, IOptions<JwtOptions> options) : ITokenService
{
    private readonly TokenRepository _repository = repository;
    private readonly JwtOptions _jwt = options.Value;

    public async Task<TokenResponse> CreateTokensAsync(ApplicationUser applicationUser)
    {
        ArgumentNullException.ThrowIfNull(applicationUser);

        var accessToken = await GenerateAccessToken(applicationUser).ConfigureAwait(false);

        var refreshToken = GenerateRefreshToken();

        await _repository.AddRefreshTokenAsync(new RefreshToken
        {
            TokenHash = HashRefreshToken(refreshToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            AgentId = applicationUser.RefreshToken is null ? applicationUser.Id : applicationUser.RefreshToken.AgentId
        }).ConfigureAwait(false);

        return new TokenResponse(AccessToken: accessToken, RefreshToken: refreshToken);
    }

    public async Task<TokenResponse> RefreshToken(RefreshTokenRequest request)
    {
        var refreshToken = await GetRefreshTokensAsync(request).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(refreshToken);
        ArgumentNullException.ThrowIfNull(refreshToken.Agent.UserName);

        if (refreshToken.IsRevoked)
            throw new InvalidOperationException("Token is revoked!");

        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            throw new InvalidOperationException("Token is expired!");

        return await CreateTokensAsync(refreshToken.Agent).ConfigureAwait(false);
    }

    public async Task<RefreshToken> GetRefreshTokensAsync(RefreshTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var tokenHash = HashRefreshToken(request.RefreshToken);

        var refreshToken = await _repository.GetRefreshTokensAsync(tokenHash).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(refreshToken);

        return refreshToken;
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {

        var user = await _repository.FindByEmailAsync(email).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(user);

        return await _repository.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
    }

    private async Task<string> GenerateAccessToken(ApplicationUser user)
    {

        if (string.IsNullOrEmpty(user.UserName))
            throw new InvalidOperationException("Create token failed: Username is required!");

        if (string.IsNullOrEmpty(user.Email))
            throw new InvalidOperationException("Create token failed: Email is required!");

        var roles = await _repository.GetRolesAsync(user).ConfigureAwait(false);

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

        return new JwtSecurityTokenHandler().WriteToken(token);
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
