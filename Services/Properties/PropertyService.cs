using RealEstate.DTOs.Properties;
using RealEstate.DTOs.Properties.Documents;
using RealEstate.DTOs.Properties.Leases;
using RealEstate.Entities.Properties;
using RealEstate.Enums.Properties;
using RealEstate.Repositories.Properties;

namespace RealEstate.Services.Properties;

interface IPropertyService
{
    Task<IEnumerable<RealEstateProperty>> GetListAsync();
    Task<RealEstateProperty?> GetAsync(Guid id);
    Task<RealEstateProperty> AddAsync(CreateDTO dto);
    Task UpdateAsync(Guid id, UpdateDTO dto);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
}

#pragma warning disable CA1515
public class PropertyService(PropertyRepository repository) : IPropertyService
{
    private readonly PropertyRepository _repository = repository;

    //private readonly ImageRepository _imageRepository = imageRepository;

    public async Task<IEnumerable<RealEstateProperty>> GetListAsync() =>
        await _repository.GetListAsync().ConfigureAwait(false);

    public async Task<RealEstateProperty?> GetAsync(Guid id) =>
         await _repository.GetAsync(id).ConfigureAwait(false);

    public async Task<RealEstateProperty> AddAsync(CreateDTO dto)
    {

        ArgumentNullException.ThrowIfNull(dto);

        var allProperties = await GetListAsync().ConfigureAwait(false);

        if (!allProperties.Any())
            dto.OrderId = 1;
        else
        {
            var lastPropertyOrderNumber = allProperties.Last().OrderId;
            dto.OrderId = lastPropertyOrderNumber + 1;
        }

        var realEstateProperty = new RealEstateProperty
        {
            OrderId = dto.OrderId,
            Title = dto.Title,
            Description = dto.Description,
            PropertyType = dto.PropertyType,
            PropertyStatus = dto.PropertyStatus,
            Price = dto.Price,
            PropertyCurrency = dto.Currency,
            YearBuilt = dto.YearBuilt,
            LandArea = dto.LandArea,
            BuildingArea = dto.BuildingArea
        };
        return await _repository.AddAsync(realEstateProperty).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, UpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existProperty = await _repository.GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(existProperty);

        existProperty.Title = string.IsNullOrEmpty(dto.Title) ? existProperty.Title : dto.Title;
        existProperty.Description = string.IsNullOrEmpty(dto.Description) ? existProperty.Description : dto.Description;
        existProperty.PropertyType = dto.PropertyType is PropertyType.NotSet ? existProperty.PropertyType : dto.PropertyType;
        existProperty.PropertyStatus = dto.PropertyStatus is PropertyStatus.NotSet ? existProperty.PropertyStatus : dto.PropertyStatus;
        existProperty.PropertyCurrency = dto.PropertyCurrency is PropertyCurrency.NotSet ? existProperty.PropertyCurrency : dto.PropertyCurrency;
        existProperty.YearBuilt = dto.YearBuilt.Equals(0) ? existProperty.YearBuilt : dto.YearBuilt;
        existProperty.Price = dto.Price.Equals(0) ? existProperty.Price : dto.Price;
        existProperty.LandArea = dto.LandArea.Equals(0.0) ? existProperty.LandArea : dto.LandArea;
        existProperty.BuildingArea = dto.BuildingArea.Equals(0.0) ? existProperty.BuildingArea : dto.BuildingArea;
        existProperty.AddressId = dto.AddressId is null ? existProperty.AddressId : dto.AddressId;
        existProperty.LocationId = dto.LocationId is null ? existProperty.LocationId : dto.LocationId;
        existProperty.OwnerId = dto.OwnerId is null ? existProperty.OwnerId : dto.OwnerId;
        existProperty.AgentId = dto.AgentId is null ? existProperty.AgentId : dto.AgentId;
        existProperty.PropertyDeedId = dto.PropertyDeedId is null ? existProperty.PropertyDeedId : dto.PropertyDeedId;
        existProperty.LeaseId = dto.LeaseId is null ? existProperty.LeaseId : dto.LeaseId;

        await _repository.UpdateAsync(existProperty).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var property = await GetAsync(id).ConfigureAwait(false);

        ArgumentNullException.ThrowIfNull(property);

        //foreach (var img in property.Images)
        //{
        //    if (img == null)
        //        continue;
        //    await _imageRepository.DeleteAsync(img).ConfigureAwait(false);
        //}

        await _repository.DeleteAsync(property).ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var properties = await GetListAsync().ConfigureAwait(false);

        //foreach (var property in properties)
        //{
        //    if (property == null)
        //        continue;
        //    foreach (var img in property.Images)
        //    {
        //        if (img == null)
        //            continue;
        //        await _imageRepository.DeleteAsync(img).ConfigureAwait(false);
        //    }
        //}
        await _repository.DeleteAllAsync().ConfigureAwait(false);
    }

}



