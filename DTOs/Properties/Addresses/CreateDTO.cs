using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Properties.Addresses;

#pragma warning disable CA1515
public class CreateDTO
{
    [DefaultValue("")]
    [Required(ErrorMessage = "Country is required!")]
    public required string Country { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "Province is required!")]
    public required string Province { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "City is required!")]
    public required string City { get; set; }

    [DefaultValue("")]
    public string? District { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "Street is required!")]
    public required string Street { get; set; }

    [DefaultValue("")]
    public string? PostalCode { get; set; }

    [DefaultValue(0)]
    public int PlatesNumber { get; set; }

    #region Relationships

    [Required(ErrorMessage = "PropertyId is required!")]
    public required Guid PropertyId { get; set; }

    #endregion
}
