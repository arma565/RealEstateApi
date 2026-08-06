using RealEstate.DTOs.Properties;
using RealEstate.Entities.Properties;
using RealEstate.Repositories.Properties;

namespace RealEstate.Services.Properties;

interface IPropertyService
{
    Task<IEnumerable<RealEstateProperty>> GetListAsync();
    Task<RealEstateProperty?> GetAsync(Guid id);
    Task<RealEstateProperty> AddAsync(RealEstatePropertyDTO realEstatePropertyDTO);
    Task UpdateAsync(Guid id , RealEstatePropertyDTO realEstatePropertyDTO);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync();
    Task<RealEstateProperty?> LastProperty();
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

    public async Task<RealEstateProperty> AddAsync(RealEstatePropertyDTO realEstatePropertyDTO)
    {

        ArgumentNullException.ThrowIfNull(realEstatePropertyDTO);

        var lastProperty = await LastProperty().ConfigureAwait(false);

        if (lastProperty == null)
            realEstatePropertyDTO.OrderId = 1;
        else
            realEstatePropertyDTO.OrderId++;

        var realEstateProperty = new RealEstateProperty
        {
            OrderId = realEstatePropertyDTO.OrderId,
            Title = realEstatePropertyDTO.Title,
            Description = realEstatePropertyDTO.Description,
            PropertyType = realEstatePropertyDTO.PropertyType,
            PropertyStatus = realEstatePropertyDTO.PropertyStatus,
            Price = realEstatePropertyDTO.Price,
            PropertyCurrency = realEstatePropertyDTO.Currency,
            YearBuilt = realEstatePropertyDTO.YearBuilt,
            LandArea = realEstatePropertyDTO.LandArea,
            BuildingArea = realEstatePropertyDTO.BuildingArea,
            AddressId = realEstatePropertyDTO.AddressId,
            LocationId = realEstatePropertyDTO.LocationId,
            OwnerId = realEstatePropertyDTO.OwnerId,
            AgentId = realEstatePropertyDTO.AgentId,
            PropertyDeedId = realEstatePropertyDTO.PropertyDeedId,
            LeaseId = realEstatePropertyDTO.LeaseId
        };
        return await _repository.AddAsync(realEstateProperty).ConfigureAwait(false);
    }

    public async Task UpdateAsync(Guid id, RealEstatePropertyDTO realEstatePropertyDTO)
    {
        ArgumentNullException.ThrowIfNull(realEstatePropertyDTO);

        var property = new RealEstateProperty
        {
            Id = id,
            OrderId = realEstatePropertyDTO.OrderId,
            Title = realEstatePropertyDTO.Title,
            Description = realEstatePropertyDTO.Description,
            PropertyType = realEstatePropertyDTO.PropertyType,
            PropertyStatus = realEstatePropertyDTO.PropertyStatus,
            Price = realEstatePropertyDTO.Price,
            PropertyCurrency = realEstatePropertyDTO.Currency,
            YearBuilt = realEstatePropertyDTO.YearBuilt,
            LandArea = realEstatePropertyDTO.LandArea,
            BuildingArea = realEstatePropertyDTO.BuildingArea,
            AddressId = realEstatePropertyDTO.AddressId,
            LocationId = realEstatePropertyDTO.LocationId,
            OwnerId = realEstatePropertyDTO.OwnerId,
            AgentId = realEstatePropertyDTO.AgentId,
            PropertyDeedId = realEstatePropertyDTO.PropertyDeedId,
            LeaseId = realEstatePropertyDTO.LeaseId
        };

        await _repository.UpdateAsync(property).ConfigureAwait(false);
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

    public async Task<RealEstateProperty?> LastProperty() =>
        (await _repository.GetListAsync().ConfigureAwait(false)).LastOrDefault();
   
}



