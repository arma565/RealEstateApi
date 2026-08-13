using RealEstate.Enums.Properties.Payments;
using System.ComponentModel;

namespace RealEstate.DTOs.Properties.Leases.Payments;

#pragma warning disable CA1515
public class UpdateDTO
{
    [DefaultValue(0.0)]
    public decimal Amount { get; set; }

    [DefaultValue(PaymentType.Deposit)]
    public PaymentType PaymentType { get; set; }

    [DefaultValue(PaymentStatus.Pending)]
    public PaymentStatus PaymentStatus { get; set; }

    #region Relationships

    public Guid LeaseId { get; set; }

    #endregion
}
