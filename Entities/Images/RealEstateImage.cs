using RealEstate.Entities.Properties.Documents;
using RealEstate.Entities.Supports;
using RealEstate.Entities.Users;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Images;

#pragma warning disable CA1515
public class RealEstateImage
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue("")]
    public string? ImageFilePath { get; set; } = default;

    [DefaultValue("")]
    public string? ThumbnailFilePath { get; set; } = default;

    #region Relationships

    public string? UserId { get; set; } = null!;
    public ApplicationUser? User { get; set; } = null!;

    public Guid? PropertyDeedId { get; set; } = null!;
    public PropertyDeed? Deed { get; set; } = null!;

    public Guid? SupportId { get; set; } = null!;
    public RealEstateSupport? Support { get; set; } = null!;
    #endregion
}
