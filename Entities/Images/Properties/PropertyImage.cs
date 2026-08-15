using RealEstate.Entities.Properties;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Images.Properties;

#pragma warning disable CA1515
public class PropertyImage : BaseImage
{
    public int OrderId { get; set; }

    public bool IsCoverImage { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }
    [JsonIgnore]
    public RealEstateProperty Property { get; set; } = null!;

    #endregion



}
