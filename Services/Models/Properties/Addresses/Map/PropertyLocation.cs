using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Models.Properties.Addresses.Map;

#pragma warning disable CA1515
public class PropertyLocation
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue(0.0)]
    public double Latitude { get; set; } = default;

    [DefaultValue(0.0)]
    public double Longitude { get; set; } = default!;

    #region Relationships

    public Guid? PropertyId { get; set; } = null!;
    public RealEstateProperty? Property { get; set; } = null!;

    #endregion

}
