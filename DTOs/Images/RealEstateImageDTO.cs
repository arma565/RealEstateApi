using System.ComponentModel;

namespace RealEstate.Entities.Images;

#pragma warning disable CA1515
public class RealEstateImageDTO
{
    [DefaultValue("")]
    public Uri? ImageFileUrl { get; set; } = default;

    [DefaultValue("")]
    public Uri? ThumbnailFileUrl { get; set; } = default;

    #region Relationships

    public string? UserId { get; set; } = null!;

    public Guid? PropertyId { get; set; } = null!;

    public Guid? PropertyDeedId { get; set; } = null!;

    public Guid? SupportId { get; set; } = null!;
    #endregion

}
