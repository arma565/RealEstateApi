using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Properties.Addresses.Map;

#pragma warning disable CA1515
public class PropertyLocation
{
    [Key]
    public Guid Id { get; set; } = new();

    public required double Latitude { get; set; }

    public required double Longitude { get; set; }

    #region Relationships
    
    public Guid PropertyId { get; set; }
    [JsonIgnore]
    public RealEstateProperty Property { get; set; } = null!;

    #endregion

}
