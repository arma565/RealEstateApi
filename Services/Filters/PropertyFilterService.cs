using RealEstate.Services.Enums.Properties;
using RealEstate.Services.Models.Properties;

namespace RealEstate.Services.Filters;

interface IPropertyFilterService {
    Task<IEnumerable<RealEstateProperty>> SortPropertyListAsync(SortType type, IEnumerable<RealEstateProperty> propertyList);
}

#pragma warning disable CA1515
public class PropertyFilterService : IPropertyFilterService
{
    public async Task<IEnumerable<RealEstateProperty>> SortPropertyListAsync(SortType type, IEnumerable<RealEstateProperty> propertyList)
    {
        return type switch
        {
            SortType.Ascending => propertyList.OrderBy(property => property.OrderId),
            SortType.Descending => propertyList.OrderByDescending(property => property.OrderId),
            SortType.DateModified => propertyList.OrderBy(property => property.OrderId),
            _ => propertyList.OrderBy(property => property.OrderId),
        };
    }

}
