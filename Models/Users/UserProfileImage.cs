#pragma warning disable CA1515
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Users;

public class UserProfileImage
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue("")]
    public string? ProfileImageName { get; set; } = default;

    [DefaultValue("")]
    public string? UserId { get; set; } = default;

    public ApplicationUser User { get; set; } = null!;
}
