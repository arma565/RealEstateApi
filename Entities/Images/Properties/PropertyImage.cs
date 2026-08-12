using RealEstate.Entities.Properties;

namespace RealEstate.Entities.Images.Properties;

#pragma warning disable CA1515
public class PropertyImage : BaseImage
{
    public int OrderId { get; set; }

    public bool IsCoverImage { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = null!;

    #endregion



}
