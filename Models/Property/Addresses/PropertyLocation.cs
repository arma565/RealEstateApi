using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Property.Locations
{
    public class PropertyLocation
    {
        public decimal Latitude { get; set; } = default!;

        public decimal Longitude { get; set; } = default!;

    }
}
