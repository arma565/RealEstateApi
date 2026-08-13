
using RealEstate.DTOs.Properties;
using RealEstate.Entities.Properties;
using RealEstate.Enums.Properties;
using RealEstate.Repositories.Properties;

namespace RealEstate.Services.Properties;

interface IPropertyService
{
    Task<IEnumerable<RealEstateProperty>> GetListAsync();
    Task<RealEstateProperty> GetAsync(Guid id);
    Task<RealEstateProperty> AddAsync(CreateDTO createDTO);
    Task UpdateAsync(Guid id, UpdateDTO updateDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PropertyService(PropertyRepository repository) : IPropertyService
{
    private readonly PropertyRepository _repository = repository;

    public async Task<IEnumerable<RealEstateProperty>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<RealEstateProperty> GetAsync(Guid id)
    {
        var property = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(property);

        return property;
    }

    public async Task<RealEstateProperty> AddAsync(CreateDTO createDTO)
    {

        ArgumentNullException.ThrowIfNull(createDTO);

        var allProperties = await GetListAsync().ConfigureAwait(false);

        if (!allProperties.Any())
            createDTO.OrderId = 1;
        else
        {
            var lastPropertyOrderNumber = allProperties.Last().OrderId;
            createDTO.OrderId = lastPropertyOrderNumber + 1;
        }

        return await _repository.AddAsync(new RealEstateProperty
        {
            OrderId = createDTO.OrderId,
            Title = createDTO.Title,
            Description = createDTO.Description,
            PropertyType = createDTO.PropertyType,
            PropertyStatus = createDTO.PropertyStatus,
            Price = createDTO.Price,
            PropertyCurrency = createDTO.Currency,
            YearBuilt = createDTO.YearBuilt,
            LandArea = createDTO.LandArea,
            BuildingArea = createDTO.BuildingArea,
            OwnerId = createDTO.OwnerId,
            AgentId = createDTO.AgentId
        }).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, UpdateDTO updateDTO)
    {
        var property = await _repository.GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(property);

        ArgumentNullException.ThrowIfNull(updateDTO);

        property.Title = string.IsNullOrEmpty(updateDTO.Title) ? property.Title : updateDTO.Title;
        property.Description = string.IsNullOrEmpty(updateDTO.Description) ? property.Description : updateDTO.Description;
        property.PropertyType = updateDTO.PropertyType is PropertyType.NotSet ? property.PropertyType : updateDTO.PropertyType;
        property.PropertyStatus = updateDTO.PropertyStatus is PropertyStatus.NotSet ? property.PropertyStatus : updateDTO.PropertyStatus;
        property.PropertyCurrency = updateDTO.PropertyCurrency is PropertyCurrency.NotSet ? property.PropertyCurrency : updateDTO.PropertyCurrency;
        property.YearBuilt = updateDTO.YearBuilt != property.YearBuilt ? updateDTO.YearBuilt : property.YearBuilt;
        property.Price = updateDTO.Price != property.Price ? updateDTO.Price : property.Price;
        property.LandArea = updateDTO.LandArea != property.LandArea ? updateDTO.LandArea : property.LandArea;
        property.BuildingArea = updateDTO.BuildingArea != property.BuildingArea ? updateDTO.BuildingArea : property.BuildingArea;
        property.OwnerId = updateDTO.OwnerId != property.OwnerId ? updateDTO.OwnerId : property.OwnerId;
        property.AgentId = updateDTO.AgentId != property.AgentId ? updateDTO.AgentId : property.AgentId;

        await _repository.UpdateAsync(property).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var property = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(property);

        await _repository.DeleteAsync(property).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync() =>
        await _repository.DeleteAllAsync().ConfigureAwait(false);
    

}



