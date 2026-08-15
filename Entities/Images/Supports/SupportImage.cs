using RealEstate.Entities.Supports;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Images.Supports;

#pragma warning disable CA1515
public class SupportImage : BaseImage
{
    #region Relationships
    
    public Guid SupportId { get; set; }
    [JsonIgnore]
    public RealEstateSupport Support { get; set; } = null!;

    #endregion

}
