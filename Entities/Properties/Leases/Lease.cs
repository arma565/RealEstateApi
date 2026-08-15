using RealEstate.Entities.Persons.Owners;
using RealEstate.Entities.Persons.Tenants;
using RealEstate.Entities.Properties.Leases.Payments;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Properties.Leases;

#pragma warning disable CA1515
public class Lease
{
    [Key]
    public Guid Id { get; set; } = new();

    public required decimal MonthlyRent { get; set; }

    public required decimal DepositAmount { get; set; }

    public TimeOnly StartTime { get; set; } = TimeOnly.Parse(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    public required TimeOnly EndTime { get; set; }

    public DateOnly StartDate { get; set; } = DateOnly.Parse(DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    public required DateOnly EndDate { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }
    [JsonIgnore]
    public RealEstateProperty Property { get; set; } = null!;

    public Guid OwnerId { get; set; }
    [JsonIgnore]
    public Owner Owner { get; set; } = null!;

    public Guid TenantId { get; set; }
    [JsonIgnore]
    public Tenant Tenant { get; set; } = null!;

    public ICollection<Payment> Payments { get; } = [];

    #endregion
}
