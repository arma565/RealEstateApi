using RealEstate.Enums.Properties;
using System.ComponentModel;

namespace RealEstate.DTOs.Properties.Features;

#pragma warning disable CA1515
public class UpdateDTO
{
    [DefaultValue("")]
    public string? Name { get; set; }

    [DefaultValue(PropertyFeatureCategory.Interior)]
    public PropertyFeatureCategory Category { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }

    #endregion
}
