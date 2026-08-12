using RealEstate.Entities.Supports;

namespace RealEstate.Entities.Images.Supports;

#pragma warning disable CA1515
public class SupportImage : BaseImage
{
    #region Relationships
    
    public Guid SupportId { get; set; }
    public RealEstateSupport Support { get; set; } = null!;

    #endregion

}
