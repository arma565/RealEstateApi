using RealEstate.Models.Property.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Property.Features;

#pragma warning disable CA1515
public class Feature
{
    [Key]
    public Guid Id { get; set; } = default;

    [DefaultValue("")]
    public string? Name { get; set; } = default;

    [DefaultValue(PropertyFeatureCategory.Interior)]
    public PropertyFeatureCategory Category { get; set; }
}
