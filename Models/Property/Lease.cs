using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstate.Models.Property
{
    public class Lease
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }

        public Guid TenantId { get; set; }

        public Guid LandlordId { get; set; }

        public decimal MonthlyRent { get; set; }

        [Required(ErrorMessage = "DepositAmount is required!")]
        public decimal DepositAmount { get; set; }

        public TimeOnly? StartTime { get; set; } = TimeOnly.Parse(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

        public DateOnly StartDate { get; set; } = DateOnly.Parse(DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

        public TimeOnly EndTime { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
