using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Models.Persons;
using RealEstate.Models.Support;
using RealEstate.Services.Images;

#pragma warning disable CA1515
namespace RealEstate.Services.Persons;

  interface IPersonRepositoryService
{
    Task<IEnumerable<Person>> GetPersonsListAsync();
    Task<Person?> GetPersonAsync(Guid id);
    Task<bool> GetPersonByPersonIDAsync(long personID);
    Task<Person> AddPersonAsync(Person newPerson);
    Task<Person> UpdatePersonAsync(Person updatePerson);
    Task DeletePersonAsync(Person deletePerson);
    Task DeleteAllPersonsAsync();
    Task<bool> IsPersonExistAsync(long personID);
}

public sealed class ProeprtyRepositoryService(AppDbContext context,
                                        ImageService imageService) : IPersonRepositoryService
{
    private readonly AppDbContext _context = context;

    private readonly ImageService _imageService = imageService;

    #region Property
    public async Task<IEnumerable<RealEstateProperty>> GetAssetListDescendingAsync() =>
          await _context
            .Assets
            .AsNoTracking()
            .Include(prop => prop.Persons)
            .Include(assetImg => assetImg.PropertyImages)
            .OrderByDescending(prop => prop.OrderID)
            .ToListAsync().ConfigureAwait(false);

    public async Task<IEnumerable<RealEstateProperty>> GetAssetListAscendingAsync() =>
        await _context
            .Assets
            .AsNoTracking()
            .Include(prop => prop.Persons)
            .Include(assetImg => assetImg.PropertyImages)
            .OrderBy(prop => prop.OrderID)
            .ToListAsync().ConfigureAwait(false);

    public async Task<IEnumerable<RealEstateProperty>> GetAssetListDateModifiedAsync() =>
        await _context
            .Assets
            .AsNoTracking()
            .Include(prop => prop.Persons)
            .Include(assetImg => assetImg.PropertyImages)
            .OrderBy(prop => prop.Date)
            .ToListAsync().ConfigureAwait(false);

    public async Task<RealEstateProperty?> GetAssetAsync(Guid assetID) =>
        await _context
            .Assets
            .AsNoTracking()
            .Include(prop => prop.Persons)
            .Include(assetImg => assetImg.PropertyImages)
            .SingleOrDefaultAsync(prop => prop.Id == assetID)
            .ConfigureAwait(false);

    public async Task<RealEstateProperty?> AddAssetAsync(RealEstateProperty newAsset)
    {
        await _context.Assets.AddAsync(newAsset).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return newAsset;
    }

    public async Task UpdateAssetAsync(RealEstateProperty asset)
    {
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAssetAsync(RealEstateProperty asset)
    {
        if (asset == null || asset.PropertyImages == null)
            return;

        var environmentPath = _imageService.GetLocalImagesFullPath("asset");
        foreach (var assetImg in asset.PropertyImages)
        {
            File.Delete(Path.Combine(environmentPath, assetImg.FileName));
        }
        _context.Assets.Remove(asset);
       await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllAssetsAsync()
    {
        var environmentPath = _imageService.GetLocalImagesFullPath("asset");
        if (Directory.Exists(environmentPath))
        {
            var filesDir = Directory.GetFiles(environmentPath);
            foreach (var file in filesDir)
            {
                File.Delete(file);
            }
        }
        await _context.Assets.ExecuteDeleteAsync().ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<RealEstateProperty?> FindAssetByPlatesNumberAsync(string platesNumber)
    {
        var assetList = await GetAssetListDescendingAsync().ConfigureAwait(false);
        return assetList.FirstOrDefault(asset => asset.PlatesNumber == platesNumber);
    }

    public async Task<bool> IsAssetExistAsync(string plateNumber) =>
       await _context.Assets.AsNoTracking().AnyAsync(prop => prop.PlatesNumber == plateNumber).ConfigureAwait(false);
    #endregion

    #region AssetImage
    public async Task<List<PropertyImage>> GetAssetImageListAsync() =>
       await _context
       .AssetImages
       .AsNoTracking()
       .OrderByDescending(assetImg => assetImg.Id)
       .ToListAsync()
       .ConfigureAwait(false);

    public async Task<PropertyImage?> GetAssetImageAsync(Guid assetImageID) =>
     await _context
    .AssetImages.AsNoTracking()
    .SingleOrDefaultAsync(assetImg => assetImg.Id == assetImageID)
    .ConfigureAwait(false);

    public async Task AddAssetImageAsync(PropertyImage assetImage)
    {
        await _context.AssetImages.AddAsync(assetImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAssetImage(PropertyImage assetImage)
    {
        if (assetImage == null || assetImage.FileName == null)
            return;
        var environmentPath = _imageService.GetLocalImagesFullPath("asset");
        var filesDir = Directory.GetFiles(environmentPath);
        foreach (var filePath in filesDir)
        {
            var assetImgPath = Path.Combine(environmentPath, assetImage.FileName);
            if (assetImgPath == filePath)
                File.Delete(filePath);
        }
        _context.AssetImages.Remove(assetImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
    #endregion

    #region Person
    public async Task<IEnumerable<Person>> GetPersonsListAsync() =>
        await _context
            .Persons.AsNoTracking()
            .OrderByDescending(per => per.Id)
            .ToListAsync().ConfigureAwait(false);

    public async Task<Person?> GetPersonAsync(Guid id) =>
        await _context.Persons.AsNoTracking().SingleOrDefaultAsync(pers => pers.Id == id).ConfigureAwait(false);

    public async Task<bool> GetPersonByPersonIDAsync(long personID) =>
        await _context.Persons.AsNoTracking().AnyAsync(pers => pers.PersonID == personID).ConfigureAwait(false);

    public async Task<Person> AddPersonAsync(Person newPerson)
    {
        await _context.Persons.AddAsync(newPerson).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return newPerson;
    }

    public async Task<Person> UpdatePersonAsync(Person updatePerson)
    {
        _context.Persons.Update(updatePerson);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return updatePerson;
    }

    public async Task DeletePersonAsync(Person deletePerson)
    {
        _context.Persons.Remove(deletePerson);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAllPersonsAsync()
    {
        _context.Persons.ExecuteDelete();
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<bool> IsPersonExistAsync(long personID) =>
      await _context.Persons.AsNoTracking().AnyAsync(pers => pers.PersonID == personID).ConfigureAwait(false);
    #endregion

    #region Support
    public async Task<IEnumerable<SupportApp>> GetSupportListAsync() =>
       await _context
      .Supports
      .AsNoTracking()
      .Include(support => support.SupportImage)
      .ToListAsync().ConfigureAwait(false);

    public async Task<SupportApp?> GetSupportAsync(Guid supportID) =>
      await _context
          .Supports.AsNoTracking()
          .Include(sups => sups.SupportImage)
          .SingleOrDefaultAsync(sup => sup.Id == supportID)
          .ConfigureAwait(false);

    public async Task<SupportApp> AddSupportAsync(SupportApp newSupport)
    {
        await _context.Supports.AddAsync(newSupport).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return newSupport;
    }

    public async Task UpdateSupportAsync(SupportApp updateSupport)
    {
        _context.Supports.Update(updateSupport);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteSupportAsync(SupportApp support)
    {
        if (support == null)
            return;

        if (support.SupportImage != null)
        {
            var environmentPath = _imageService.GetLocalImagesFullPath("support");
            var filesDir = Directory.GetFiles(environmentPath);
            var supImagePath = Path.Combine(environmentPath, support.SupportImage.SupportImageFileName);
            foreach (var filePath in filesDir)
            {
                if (supImagePath == filePath)
                    File.Delete(filePath);
            }
        }
        _context.Supports.Remove(support);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
    public async Task DeleteAllSupportsAsync()
    {
        var environmentPath = _imageService.GetLocalImagesFullPath("support");
        if (Directory.Exists(environmentPath))
        {
            var filesPath = Directory.GetFiles(environmentPath);
            foreach (var filePath in filesPath)
            {
                File.Delete(filePath);
            }
        }
        _context.Supports.ExecuteDelete();
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
    #endregion

    #region SupportImage
    public async Task<List<SupportImage>> GetSupportImageListAsync() =>
      await _context
      .SupportImages
      .ToListAsync()
      .ConfigureAwait(false);

    public async Task<SupportImage?> GetSupportImageAsync(Guid supportImageID) =>
       await _context
      .SupportImages.AsNoTracking()
      .SingleOrDefaultAsync(supImage => supImage.Id == supportImageID)
      .ConfigureAwait(false);

    public async Task AddSupportImageAsync(SupportImage supportImage)
    {
        await _context.SupportImages.AddAsync(supportImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateSupportImageAsync(SupportImage supportImage)
    {
        _context.SupportImages.Update(supportImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteSupportImageAsync(SupportImage supportImage)
    {
        if (supportImage == null || supportImage.SupportImageFileName == null)
            return;
        var environmentPath = _imageService.GetLocalImagesFullPath("support");
        var supImgPath = Path.Combine(environmentPath, supportImage.SupportImageFileName);
        var filesPath = Directory.GetFiles(environmentPath);
        foreach (var filePath in filesPath)
        {
            if(supImgPath == filePath)
                File.Delete(filePath);
        }
        _context.SupportImages.Remove(supportImage);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
    #endregion

}



