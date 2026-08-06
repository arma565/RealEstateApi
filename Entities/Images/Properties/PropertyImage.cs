using RealEstate.Entities.Images;
using RealEstate.Entities.Properties;
using System.ComponentModel;

namespace RealEstate.Entities.Images.Properties;

#pragma warning disable CA1515
public class PropertyImage : RealEstateImage
{
    [DefaultValue(0)]
    public int Order { get; set; } = default;

    [DefaultValue(false)]
    public bool IsCoverImage { get; set; } = default;

    #region Relationships

    public Guid? PropertyId { get; set; } = null!;
    public RealEstateProperty? Property { get; set; } = null!;

    #endregion
}
