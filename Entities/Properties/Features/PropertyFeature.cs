using RealEstate.Entities.Properties;
using RealEstate.Enums.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Properties.Features;

#pragma warning disable CA1515
public class PropertyFeature
{
    [Key]
    public Guid Id { get; set; } = default;

    [DefaultValue("")]
    public string? Name { get; set; } = default;

    [DefaultValue(PropertyFeatureCategory.Interior)]
    public PropertyFeatureCategory PropertyFeatureCategory { get; set; }

    #region Relationships

    public Guid? PropertyId { get; set; } = null!;
    public RealEstateProperty? Property { get; set; } = null!;

    #endregion
}
