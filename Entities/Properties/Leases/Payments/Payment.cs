using RealEstate.Entities.Properties.Leases;
using RealEstate.Enums.Properties.Payments;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstate.Entities.Properties.Leases.Payments;

#pragma warning disable CA1515
public class Payment
{
    [Key]
    public Guid Id { get; set; } = new();

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; } = DateTime.Parse(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    public PaymentType PaymentType { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    #region Relationships

    public Guid LeaseId { get; set; }
    public Lease Lease { get; set; } = null!;

    #endregion
}
