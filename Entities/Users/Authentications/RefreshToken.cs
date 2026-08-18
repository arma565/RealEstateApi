using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Users.Authentications;

#pragma warning disable CA1515
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = new();

    public string? TokenHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    #region Relationships

    public string? AgentId { get; set; }

    public ApplicationUser Agent { get; set; } = null!;

    #endregion
}
