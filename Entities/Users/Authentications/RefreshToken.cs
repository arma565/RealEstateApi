namespace RealEstate.Entities.Users.Authentications;

#pragma warning disable CA1515
public class RefreshToken
{
    public int Id { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; } = default!;

    public bool IsRevoked { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = default!;

    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}
