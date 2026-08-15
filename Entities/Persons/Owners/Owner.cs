using RealEstate.Entities.Properties;
using RealEstate.Entities.Properties.Leases;

namespace RealEstate.Entities.Persons.Owners;

#pragma warning disable CA1515
public class Owner() : Person{

    public ICollection<Lease> Leases { get; } = [];

    public ICollection<RealEstateProperty> RealEstateProperties { get; } = [];

}

