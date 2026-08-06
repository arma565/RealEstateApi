using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Entities.Properties;
using RealEstate.Repositories.Images;

namespace RealEstate.Repositories.Properties;

interface IPropertyRepository
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
public class PropertyRepository(AppDbContext context,
                                        ImageRepository imageRepository) : IPropertyRepository
{
    private readonly AppDbContext _context = context;

    private readonly ImageRepository _imageRepository = imageRepository;

    public async Task<IEnumerable<RealEstateProperty>> GetListAsync() =>
         await _context
            .Properties
            .AsNoTracking()
            .Include(property => property.Address)
            .Include(property => property.Location)
            .Include(property => property.Owner)
            .Include(property => property.PropertyDeed)
            .Include(property => property.Lease)
            .Include(property => property.Features)
            .Include(propertyImg => propertyImg.Images)
            .ToListAsync()
            .ConfigureAwait(false);

    public async Task<RealEstateProperty?> GetAsync(Guid id) =>
        await _context
            .Properties
            .AsNoTracking()
             .Include(property => property.Address)
            .Include(property => property.Location)
            .Include(property => property.Owner)
            .Include(property => property.PropertyDeed)
            .Include(property => property.Lease)
            .Include(property => property.Features)
            .Include(propertyImg => propertyImg.Images)
            .SingleOrDefaultAsync(property => property.Id == id)
            .ConfigureAwait(false);

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
            PlatesNumber = realEstatePropertyDTO.PlatesNumber,
            PropertyType = realEstatePropertyDTO.PropertyType,
            PropertyStatus = realEstatePropertyDTO.PropertyStatus,
            Price = realEstatePropertyDTO.Price,
            Currency = realEstatePropertyDTO.Currency,
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
        await _context.Properties.AddAsync(realEstateProperty).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return realEstateProperty;
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
            PlatesNumber = realEstatePropertyDTO.PlatesNumber,
            PropertyType = realEstatePropertyDTO.PropertyType,
            PropertyStatus = realEstatePropertyDTO.PropertyStatus,
            Price = realEstatePropertyDTO.Price,
            Currency = realEstatePropertyDTO.Currency,
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

        _context.Properties.Update(property);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var property = await GetAsync(id).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(property);

        foreach (var img in property.Images)
        {
            await _imageRepository.DeleteAsync(img.Id).ConfigureAwait(false);
        }

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAsync()
    {
        var properties = await GetListAsync().ConfigureAwait(false);

        foreach (var property in properties)
        {
            if (property == null)
                continue;
            foreach (var img in property.Images)
            {
                await _imageRepository.DeleteAsync(img.Id).ConfigureAwait(false);
            }
        }
        await _context.Properties.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<RealEstateProperty?> LastProperty() =>
        (await GetListAsync().ConfigureAwait(false)).LastOrDefault();
   
}



