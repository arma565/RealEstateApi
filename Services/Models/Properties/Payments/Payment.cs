using RealEstate.Services.Enums.Properties.Payments;
using RealEstate.Services.Models.Properties.Leases;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstate.Services.Models.Properties.Payments;

#pragma warning disable CA1515
public class Payment
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue(0.0)]
    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.Parse(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    [DefaultValue(PaymentType.Deposit)]
    public PaymentType PaymentType { get; set; }

    [DefaultValue(PaymentStatus.Pending)]
    public PaymentStatus PaymentStatus { get; set; }

    #region Relationships

    public Guid? LeaseId { get; set; } = null!;
    public Lease? Lease { get; set; } = null!;

    #endregion
}
