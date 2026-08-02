using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Models.Properties.Addresses;

#pragma warning disable CA1515
public class PropertyAddressDTO
{
    [Required(ErrorMessage = "Country is required!")]
    public string? Country { get; set; } = default;

    [Required(ErrorMessage = "Province is required!")]
    public string? Province { get; set; } = default;

    [Required(ErrorMessage = "City is required!")]
    public string? City { get; set; } = default;

    [DefaultValue("")]
    public string? District { get; set; } = default!;

    [Required(ErrorMessage = "Street is required!")]
    public string? Street { get; set; } = default;

    [Required(ErrorMessage = "PlatesNumber is required!")]
    public int PlatesNumber { get; set; } = default;

    [DefaultValue("")]
    public string? PostalCode { get; set; } = default!;

    #region Relationships

    public Guid? PropertyId { get; set; } = null!;

    #endregion
}
