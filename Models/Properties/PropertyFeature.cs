using RealEstate.Enums.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Property;

#pragma warning disable CA1515
public class PropertyFeature
{
    [Key]
    public Guid Id { get; set; } = default;

    [DefaultValue("")]
    public string? Name { get; set; } = default;

    [DefaultValue(PropertyFeatureCategory.Interior)]
    public PropertyFeatureCategory Category { get; set; }

    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = null!;
}
