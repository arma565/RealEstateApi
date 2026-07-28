using RealEstate.Enums.Properties.Payment;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstate.Models.Property;

#pragma warning disable CA1515
public class Payment
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue(0.0)]
    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.Parse(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    [DefaultValue(PaymentType.Deposit)]
    public PaymentType Type { get; set; }

    [DefaultValue(PaymentStatus.Pending)]
    public PaymentStatus Status { get; set; }

    public Guid LeaseId { get; set; }
    public Lease Lease { get; set; } = default!;
}
