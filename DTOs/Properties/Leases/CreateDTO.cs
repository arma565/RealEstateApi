using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Properties.Leases;

#pragma warning disable CA1515
public class CreateDTO
{
    [DefaultValue(0.0)]
    [Required(ErrorMessage = "MonthlyRent is required!")]
    public required decimal MonthlyRent { get; set; } 

    [DefaultValue(0.0)]
    [Required(ErrorMessage = "DepositAmount is required!")]
    public required decimal DepositAmount { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "EndTime is required!")]
    public required TimeOnly EndTime { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "EndDate is required!")]
    public required DateOnly EndDate { get; set; }

    #region Relationships

    [Required(ErrorMessage = "PropertyId is required!")]
    public required Guid PropertyId { get; set; }

    [Required(ErrorMessage = "OwnerId is required!")]
    public required Guid OwnerId { get; set; }

    [Required(ErrorMessage = "TenantId is required!")]
    public required Guid TenantId { get; set; }

    #endregion
}
