using System.ComponentModel;

namespace RealEstate.DTOs.Properties.Leases;

#pragma warning disable CA1515
public class UpdateDTO
{
    [DefaultValue(0.0)]
    public decimal MonthlyRent { get; set; }

    [DefaultValue(0.0)]
    public decimal DepositAmount { get; set; }

    [DefaultValue("")]
    public TimeOnly EndTime { get; set; }

    [DefaultValue("")]
    public DateOnly EndDate { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }

    public Guid OwnerId { get; set; }

    public Guid TenantId { get; set; }

    #endregion
}
