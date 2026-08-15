using RealEstate.Entities.Properties.Documents;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Images.Documents;

#pragma warning disable CA1515
public class PropertyDeedImage : BaseImage
{

    #region Relationships

    public Guid PropertyDeedId { get; set; }
    [JsonIgnore]
    public PropertyDeed PropertyDeed { get; set; } = null!;
    
    #endregion
}
