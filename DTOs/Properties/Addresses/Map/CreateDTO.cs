using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Properties.Addresses.Map;

#pragma warning disable CA1515
public class CreateDTO
{
    
    [DefaultValue(0.0)]
    [Required(ErrorMessage = "Latitude is required!")]
    public required double Latitude { get; set; }

    [DefaultValue(0.0)]
    [Required(ErrorMessage = "Longitude is required!")]
    public required double Longitude { get; set; }

    #region Relationships

    [Required(ErrorMessage = "PropertyId is required!")]
    public required Guid PropertyId { get; set; }

    #endregion

}
