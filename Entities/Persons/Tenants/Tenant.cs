using RealEstate.Entities.Properties.Leases;

namespace RealEstate.Entities.Persons.Tenants;

#pragma warning disable CA1515
public class Tenant() : Person
{
    #region Relationships

    public ICollection<Lease> Leases { get; } = [];

    #endregion
}

