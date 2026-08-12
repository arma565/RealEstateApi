using RealEstate.Entities.Properties.Documents;

namespace RealEstate.Entities.Images.Documents;

#pragma warning disable CA1515
public class PropertyDeedImage : BaseImage
{

    #region Relationships

    public Guid PropertyDeedId { get; set; }
    public PropertyDeed PropertyDeed { get; set; } = null!;
    
    #endregion
}
