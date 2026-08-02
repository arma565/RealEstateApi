using RealEstate.Services.Enums.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Models.Properties.Features;

#pragma warning disable CA1515
public class PropertyFeature
{
    [Key]
    public Guid Id { get; set; } = default;

    [DefaultValue("")]
    public string? Name { get; set; } = default;

    [DefaultValue(PropertyFeatureCategory.Interior)]
    public PropertyFeatureCategory Category { get; set; }

    #region Relationships

    public Guid? PropertyId { get; set; } = null!;
    public RealEstateProperty? Property { get; set; } = null!;

    #endregion
}
