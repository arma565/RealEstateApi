using RealEstate.Enums.Properties.Payments;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Properties.Leases.Payments;

#pragma warning disable CA1515
public class CreateDTO
{
    [DefaultValue(0.0)]
    [Required(ErrorMessage = "Amount is required!")]
    public required decimal Amount { get; set; }

    [DefaultValue(PaymentType.Deposit)]
    public PaymentType PaymentType { get; set; }

    [DefaultValue(PaymentStatus.Pending)]
    public PaymentStatus PaymentStatus { get; set; }

    #region Relationships

    [Required(ErrorMessage = "LeaseId is required!")]
    public required Guid LeaseId { get; set; }

    #endregion
}
