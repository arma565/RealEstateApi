using Microsoft.EntityFrameworkCore;
using RealEstate.Data;
using RealEstate.Models.Assets;
using RealEstate.Models.Persons;
using RealEstate.Models.Support;
using RealEstate.Services.Images;

#pragma warning disable CA1515
namespace RealEstate.Services.Assets;

  interface IAssetRepositoryService
{
    #region Asset
    Task<IEnumerable<Asset>> GetAssetListDescendingAsync();
    Task<IEnumerable<Asset>> GetAssetListAscendingAsync();
    Task<IEnumerable<Asset>> GetAssetListDateModifiedAsync();
    Task<Asset?> GetAssetAsync(Guid assetID);
    Task<Asset?> AddAssetAsync(Asset newAsset);
    Task UpdateAssetAsync(Asset asset);
    Task DeleteAssetAsync(Asset asset);
    Task DeleteAllAssetsAsync();
    Task<Asset?> FindAssetByPlatesNumberAsync(string platesNumber);
    Task<bool> IsAssetExistAsync(string plateNumber);
    #endregion
    #region AssetImage
    Task<List<AssetImage>> GetAssetImageListAsync();
    Task<AssetImage?> GetAssetImageAsync(Guid assetImageID);
    Task AddAssetImageAsync(AssetImage assetImage);
    Task DeleteAssetImage(AssetImage assetImage);
    #endregion
    #region Person
    Task<IEnumerable<Person>> GetPersonsListAsync();
    Task<Person?> GetPersonAsync(Guid id);
    Task<bool> GetPersonByPersonIDAsync(long personID);
    Task<Person> AddPersonAsync(Person newPerson);
    Task<Person> UpdatePersonAsync(Person updatePerson);
    Task DeletePersonAsync(Person deletePerson);
    Task DeleteAllPersonsAsync();
    Task<bool> IsPersonExistAsync(long personID);
    #endregion
}

public sealed class AssetRepositoryService(AppDbContext context,
                                        ImageService imageService) : IAssetRepositoryService
{
    private readonly AppDbContext _context = context;

    private readonly ImageService _imageService = imageService;

    #region Asset
    public async Task<IEnumerable<Asset>> GetAssetListDescendingAsync() =>
          await _context
            .Assets
            .AsNoTracking()
            .Include(prop => prop.Persons)
            .Include(assetImg => assetImg.AssetImages)
            .OrderByDescending(prop => prop.OrderID)
            .ToListAsync().ConfigureAwait(false);

    public async Task<IEnumerable<Asset>> GetAssetListAscendingAsync() =>
        await _context
            .Assets
            .AsNoTracking()
            .Include(prop => prop.Persons)
            .Include(assetImg => assetImg.AssetImages)
            .OrderBy(prop => prop.OrderID)
            .ToListAsync().ConfigureAwait(false);

    public async Task<IEnumerable<Asset>> GetAssetListDateModifiedAsync() =>
        await _context
            .Assets
            .AsNoTracking()
            .Include(prop => prop.Persons)
            .Include(assetImg => assetImg.AssetImages)
            .OrderBy(prop => prop.Date)
            .ToListAsync().ConfigureAwait(false);

    public async Task<Asset?> GetAssetAsync(Guid assetID) =>
        await _context
            .Assets
            .AsNoTracking()
            .Include(prop => prop.Persons)
            .Include(assetImg => assetImg.AssetImages)
            .SingleOrDefaultAsync(prop => prop.Id == assetID)
            .ConfigureAwait(false);

    public async Task<Asset?> AddAssetAsync(Asset newAsset)
    {
        await _context.Assets.AddAsync(newAsset).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
        return newAsset;
    }

    public async Task UpdateAssetAsync(Asset asset)
    {
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAssetAsync(Asset asset)
    {
        if (asset == null || asset.AssetImages == null)
            return;

        var environmentPath = _imageService.GetLocalImagesFullPath("asset");
        foreach (var assetImg in asset.AssetImages)
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

    public async Task<Asset?> FindAssetByPlatesNumberAsync(string platesNumber)
    {
        var assetList = await GetAssetListDescendingAsync().ConfigureAwait(false);
        return assetList.FirstOrDefault(asset => asset.PlatesNumber == platesNumber);
    }

    public async Task<bool> IsAssetExistAsync(string plateNumber) =>
       await _context.Assets.AsNoTracking().AnyAsync(prop => prop.PlatesNumber == plateNumber).ConfigureAwait(false);
    #endregion

    #region AssetImage
    public async Task<List<AssetImage>> GetAssetImageListAsync() =>
       await _context
       .AssetImages
       .AsNoTracking()
       .OrderByDescending(assetImg => assetImg.Id)
       .ToListAsync()
       .ConfigureAwait(false);

    public async Task<AssetImage?> GetAssetImageAsync(Guid assetImageID) =>
     await _context
    .AssetImages.AsNoTracking()
    .SingleOrDefaultAsync(assetImg => assetImg.Id == assetImageID)
    .ConfigureAwait(false);

    public async Task AddAssetImageAsync(AssetImage assetImage)
    {
        await _context.AssetImages.AddAsync(assetImage).ConfigureAwait(false);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAssetImage(AssetImage assetImage)
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



