using System.ComponentModel;

namespace RealEstate.Services.Models.Properties.Addresses.Map;

#pragma warning disable CA1515
public class PropertyLocationDTO
{
    [DefaultValue(0.0)]
    public double Latitude { get; set; } = default;

    [DefaultValue(0.0)]
    public double Longitude { get; set; } = default!;

    #region Relationships

    public Guid? PropertyId { get; set; } = null!;

    #endregion

}
