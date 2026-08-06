using System.ComponentModel;

namespace RealEstate.Entities.Images.Properties;

#pragma warning disable CA1515
public class PropertyImageDTO
{
    [DefaultValue("")]
    public string? ImageFilePath { get; set; } = default;

    [DefaultValue("")]
    public string? ThumbnailFilePath { get; set; } = default;

    [DefaultValue(0)]
    public int Order { get; set; } = default;

    [DefaultValue(false)]
    public bool IsCoverImage { get; set; } = default;

    #region Relationships

    public Guid PropertyId { get; set; }

    #endregion
}
