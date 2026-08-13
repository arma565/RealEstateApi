using RealEstate.Enums.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Properties.Features;

#pragma warning disable CA1515
public class CreateDTO
{
    [DefaultValue("")]
    [Required(ErrorMessage = "Name is required!")]
    public required string Name { get; set; }

    [DefaultValue(PropertyFeatureCategory.Interior)]
    public PropertyFeatureCategory Category { get; set; }

    #region Relationships

    [Required(ErrorMessage = "PropertyId is required!")]
    public required Guid PropertyId { get; set; }

    #endregion
}
