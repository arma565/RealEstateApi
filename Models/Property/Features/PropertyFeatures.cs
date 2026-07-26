using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Property.Features
{
    public class PropertyFeatures
    {
        [Key]
        public Guid PropertyFeaturesId { get; set; } = default;
        
        public Feature Feature { get; set; } = null!;

        public Guid PropertyId { get; set; }

        public RealEstateProperty Property { get; set; } = null!;
    }
}
