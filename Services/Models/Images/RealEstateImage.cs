using RealEstate.Services.Models.Properties;
using RealEstate.Services.Models.Properties.Documents;
using RealEstate.Services.Models.Supports;
using RealEstate.Services.Models.Users;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Models.Images;

#pragma warning disable CA1515
public class RealEstateImage
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue("")]
    public Uri? ImageFileUrl { get; set; } = default;

    [DefaultValue("")]
    public Uri? ThumbnailFileUrl { get; set; } = default;

    [DefaultValue(true)]
    public bool IsCoverImage { get; set; } = default;

    #region Relationships

    public string? UserId { get; set; } = null!;
    public ApplicationUser? User { get; set; } = null!;

    public Guid? PropertyId { get; set; } = null!;
    public RealEstateProperty? Property { get; set; } = null!;

    public Guid? PropertyDeedId { get; set; } = null!;
    public PropertyDeed? Deed { get; set; } = null!;

    public Guid? SupportId { get; set; } = null!;
    public RealEstateSupport? Support { get; set; } = null!;
    #endregion
}
