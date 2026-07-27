using RealEstate.Models.Enums.Persons;
using RealEstate.Models.Persons;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstate.Models.Property;

#pragma warning disable CA1515
public class Lease
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue(0.0)]
    public decimal MonthlyRent { get; set; } = default;

    [Required(ErrorMessage = "DepositAmount is required!")]
    public decimal DepositAmount { get; set; } = default;

    public TimeOnly? StartTime { get; set; } = TimeOnly.Parse(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    public DateOnly StartDate { get; set; } = DateOnly.Parse(DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    public TimeOnly EndTime { get; set; }

    public DateOnly EndDate { get; set; }

    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = null!;

    public ICollection<Person> Persons => []; 

    public ICollection<Payment> Payments => [];
}
