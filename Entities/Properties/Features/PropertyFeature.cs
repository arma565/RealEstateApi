using RealEstate.Enums.Properties;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Properties.Features;

#pragma warning disable CA1515
public class PropertyFeature
{
    [Key]
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public PropertyFeatureCategory PropertyFeatureCategory { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }
    [JsonIgnore]
    public RealEstateProperty Property { get; set; } = null!;

    #endregion
}
