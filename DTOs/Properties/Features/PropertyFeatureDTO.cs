using RealEstate.Enums.Properties;
using System.ComponentModel;

namespace RealEstate.Entities.Properties.Features;

#pragma warning disable CA1515
public class PropertyFeatureDTO
{
    [DefaultValue("")]
    public string? Name { get; set; } = default;

    [DefaultValue(PropertyFeatureCategory.Interior)]
    public PropertyFeatureCategory Category { get; set; }

    #region Relationships

    public Guid? PropertyId { get; set; } = null!;

    #endregion
}
