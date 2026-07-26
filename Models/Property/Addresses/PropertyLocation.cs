using System.ComponentModel;

namespace RealEstate.Models.Property.Addresses;

#pragma warning disable CA1515
public class PropertyLocation
{
    [DefaultValue(0.0)]
    public double Latitude { get; set; } = default;

    [DefaultValue(0.0)]
    public double Longitude { get; set; } = default!;

}
