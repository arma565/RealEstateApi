using System.ComponentModel;

namespace RealEstate.DTOs.Properties.Addresses.Map;

#pragma warning disable CA1515
public class UpdateDTO
{
    [DefaultValue(0.0)]
    public double Latitude { get; set; }

    [DefaultValue(0.0)]
    public double Longitude { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }

    #endregion

}
