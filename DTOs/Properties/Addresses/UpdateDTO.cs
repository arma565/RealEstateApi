using System.ComponentModel;

namespace RealEstate.DTOs.Properties.Addresses;

#pragma warning disable CA1515
public class UpdateDTO
{
    [DefaultValue("")]
    public string? Country { get; set; }

    [DefaultValue("")]
    public string? Province { get; set; }

    [DefaultValue("")]
    public string? City { get; set; }

    [DefaultValue("")]
    public string? District { get; set; }

    [DefaultValue("")]
    public string? Street { get; set; }

    [DefaultValue(0)]
    public int PlatesNumber { get; set; }

    [DefaultValue("")]
    public string? PostalCode { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }

    #endregion
}
