using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Properties.Documents;

#pragma warning disable CA1515
public class CreateDTO
{
    [DefaultValue("")]
    [Required(ErrorMessage = "DeedNumber is required!")]
    public required string DeedNumber { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "RegistryNumber is required!")]
    public required string RegistryNumber { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "IssuedBy is required!")]
    public required string IssuedBy { get; set; }

    #region Relationships

    [Required(ErrorMessage = "PropertyId is required!")]
    public required Guid PropertyId { get; set; }

    #endregion
}
