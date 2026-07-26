using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Property.Locations
{
    public class PropertyAddress
    {
        [Required(ErrorMessage = "Country is required!")]
        public string? Country { get; set; } = default;

        [Required(ErrorMessage = "Province is required!")]
        public string? Province { get; set; } = default;

        [Required(ErrorMessage = "City is required!")]
        public string? City { get; set; } = default;

        public string? District { get; set; } = default!;

        [Required(ErrorMessage = "Street is required!")]
        public string? Street { get; set; } = default;

        [Required(ErrorMessage = "PlatesNumber is required!")]
        public int PlatesNumber { get; set; } = default;

        public string? PostalCode { get; set; } = default!;
    }
}
