using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Models.Assets;
using RealEstate.Models.Persons;
using RealEstate.Services.Assets;
using RealEstate.Services.Images;
using System.Globalization;
using System.Security;
using System.Threading.Tasks;

#pragma warning disable CA1515
namespace RealEstate.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AssetController(AssetRepositoryService service, ImageService imageService) : ControllerBase
{
    private readonly AssetRepositoryService _service = service;
    private readonly ImageService _imageService = imageService;

    #region Asset
    /// <summary>
    /// Uploads one or more images for a specific asset.
    /// </summary>
    /// <param name="assetID">The GUID of the asset to associate images with.</param>
    /// <param name="images">Array of image files to upload.</param>
    /// <returns>Returns a list of uploaded image URLs if successful.</returns>
    [HttpPost("asset/upload/{assetID}")]
    public async Task<IActionResult> UploadAssetImages(string assetID, [FromForm] IFormFile[] images)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assetID))
                return BadRequest("AssetID is empty!");

            if (!Guid.TryParse(assetID, out Guid realEstateAssetID))
                return BadRequest("AssetID must be a valid GUID!");

            var asset = await _service.GetAssetAsync(realEstateAssetID).ConfigureAwait(false);

            if (asset == null)
                return NotFound("No such asset found!");

            if (images == null || images.Length == 0)
                return BadRequest("Image list cannot be empty!");

            var imageUrlList = await _imageService.UploadImages(images).ConfigureAwait(false);

            foreach (var img in imageUrlList)
            {
                var assetImage = new PropertyImage()
                {
                    FileName = img,
                    AssetID = realEstateAssetID
                };
                await _service.AddAssetImageAsync(assetImage).ConfigureAwait(false);
            }

            return Ok("Images uploaded successfully");
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
    }

    /// <summary>
    /// Downloads an asset image by its file name.
    /// </summary>
    /// <param name="imageFileName">The file name of the image to download.</param>
    /// <returns>Returns the image file if found.</returns>
    [HttpGet("asset/download/{imageFileName}")]
    public async Task<IActionResult> DownloadAssetImage(string imageFileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageFileName))
                return BadRequest("Image file name is empty!");

            List<PropertyImage> assetImagesList = await _service.GetAssetImageListAsync().ConfigureAwait(false);

            var assetImage = assetImagesList.FirstOrDefault(assetImg => assetImg.FileName == imageFileName);

            if (assetImage == null)
                return NotFound("No such image found!");

            var environmentPath = _imageService.GetLocalImagesFullPath("asset");

            var fullPath = Path.Combine(environmentPath, assetImage.FileName);

            var normalizedPath = Path.GetFullPath(fullPath);
            var basePath = Path.GetFullPath(environmentPath);
            if (!normalizedPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid file path access.");

            if (!System.IO.File.Exists(fullPath))
                return NotFound("Image file not found!");

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fullPath, out var contentType))
                contentType = "application/octet-stream";

            return PhysicalFile(fullPath, contentType, Path.GetFileName(fullPath));
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
    }

    /// <summary>
    /// Gets the list of assets in descending order.
    /// </summary>
    /// <returns>Returns a list of assets ordered descendingly.</returns>
    [HttpGet("asset/desc")]
    public async Task<ActionResult<IEnumerable<RealEstateProperty>>> GetAssetListDescending() => Ok(await _service.GetAssetListDescendingAsync().ConfigureAwait(false));

    /// <summary>
    /// Gets the list of assets in ascending order.
    /// </summary>
    /// <returns>Returns a list of assets ordered ascendingly.</returns>
    [HttpGet("asset/asc")]
    public async Task<ActionResult<IEnumerable<RealEstateProperty>>> GetAssetListAscending() => Ok(await _service.GetAssetListAscendingAsync().ConfigureAwait(false));

    /// <summary>
    /// Gets the list of assets ordered by date modified.
    /// </summary>
    /// <returns>Returns a list of assets ordered by date modified.</returns>
    [HttpGet("asset/date")]
    public async Task<ActionResult<IEnumerable<RealEstateProperty>>> GetAssetListDateModified() => Ok(await _service.GetAssetListDateModifiedAsync().ConfigureAwait(false));

    /// <summary>
    /// Gets a specific asset by its ID.
    /// </summary>
    /// <param name="assetID">The GUID of the asset.</param>
    /// <returns>Returns the asset if found.</returns>
    [HttpGet("asset/{assetID}")]
    public async Task<ActionResult<RealEstateProperty>> GetAsset(string assetID)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assetID))
                return BadRequest("AssetID is empty!");

            if (!Guid.TryParse(assetID, out Guid realEstateAssetID))
                return BadRequest("AssetID must be a valid GUID!");

            RealEstateProperty? asset = await _service.GetAssetAsync(realEstateAssetID).ConfigureAwait(false);

            if (asset is null)
                return NotFound("No such asset found!");

            return Ok(asset);

        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
    }

    /// <summary>
    /// Adds a new asset.
    /// </summary>
    /// <param name="newAsset">The asset object to add.</param>
    /// <returns>Returns the created asset with its location.</returns>
    [HttpPost("asset/add")]
    public async Task<IActionResult> AddAsset([FromBody] RealEstateProperty newAsset)
    {
        try
        {
            if (newAsset == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            switch (true)
            {
                case bool _ when string.IsNullOrWhiteSpace(newAsset.Date):
                    {
                        newAsset.Date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    break;
                case bool _ when string.IsNullOrWhiteSpace(newAsset.Time):
                    {
                        newAsset.Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    break;

                case bool _ when string.IsNullOrWhiteSpace(newAsset.Time) && string.IsNullOrWhiteSpace(newAsset.Date):
                    {
                        newAsset.Date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        newAsset.Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    break;
                default:
                    {
                        newAsset.Date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        newAsset.Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    break;
            }

            bool? isAssetExists = await _service.IsAssetExistAsync(newAsset.PlatesNumber!).ConfigureAwait(false);

            if (isAssetExists == true)
                return BadRequest("Asset is already exist!");

            var allAssets = await _service.GetAssetListAscendingAsync().ConfigureAwait(false);

            if (allAssets != null && allAssets.Any())
            {
                var lastAssetOrderID = allAssets.Last().OrderID;
                newAsset.OrderID = lastAssetOrderID + 1;
            }
            else newAsset.OrderID = 1;


            RealEstateProperty? addedAsset = await _service.AddAssetAsync(newAsset).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsset), new { assetID = newAsset.Id }, newAsset);
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }

    }

    /// <summary>
    /// Updates an existing asset.
    /// </summary>
    /// <param name="updateAsset">The asset object with updated values.</param>
    /// <returns>Returns no content if update is successful.</returns>
    [HttpPut("asset/update")]
    public async Task<IActionResult> UpdateAsset([FromBody] RealEstateProperty updateAsset)
    {
        try
        {
            if (updateAsset == null)
                return BadRequest("Failed to retrieve parameter!");

            RealEstateProperty? asset = await _service.GetAssetAsync(updateAsset.Id).ConfigureAwait(false);

            if (asset is null)
                return NotFound("No such asset found!");

            updateAsset.Date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            updateAsset.Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
            updateAsset.OrderID = asset.OrderID;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAssetAsync(updateAsset).ConfigureAwait(false);

            return NoContent();
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }

    }

    /// <summary>
    /// Deletes a specific asset by its ID.
    /// </summary>
    /// <param name="assetID">The GUID of the asset to delete.</param>
    /// <returns>Returns a success message if deleted.</returns>
    [HttpDelete("asset/delete/{assetID}")]
    public async Task<IActionResult> DeleteAsset(string assetID)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assetID))
                return BadRequest("AssetID is empty!");

            if (!Guid.TryParse(assetID, out Guid realEstateID))
                return BadRequest("Id must be a valid GUID!");

            RealEstateProperty? asset = await _service.GetAssetAsync(realEstateID).ConfigureAwait(false);

            if (asset is null)
                return NotFound("No such asset found!");

            await _service.DeleteAssetAsync(asset).ConfigureAwait(false);

            return Ok("Asset successfully deleted");
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
    }

    /// <summary>
    /// Deletes all assets.
    /// </summary>
    /// <returns>Returns a success message after deleting all assets.</returns>
    [HttpDelete("asset/delete-all")]
    public async Task<IActionResult> DeleteAllAssets()
    {
        await _service.DeleteAllAssetsAsync().ConfigureAwait(false);
        return Ok("All assets has been deleted!");
    }
    #endregion

    #region AssetImage
    /// <summary>
    /// Deletes a specific asset image by its ID.
    /// </summary>
    /// <param name="assetImageID">The GUID of the asset image to delete.</param>
    /// <returns>Returns a success message if deleted, or an error message if not found or invalid.</returns>
    [HttpDelete("asset/assetImage/delete/{assetImageID}")]
    public async Task<IActionResult> DeleteAssetImage(string assetImageID)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(assetImageID))
                return BadRequest("AssetImageID is empty!");

            if (!Guid.TryParse(assetImageID, out Guid realEstateAssetImageID))
                return BadRequest("AssetImageID must be a valid GUID!");

            PropertyImage? assetImage = await _service.GetAssetImageAsync(realEstateAssetImageID).ConfigureAwait(false);

            if (assetImage is null)
                return NotFound("No such assetImage found!");

            await _service.DeleteAssetImage(assetImage).ConfigureAwait(false);

            return NoContent();
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
    }
    #endregion

    #region Person
    /// <summary>
    /// Gets the list of all persons.
    /// </summary>
    /// <returns>Returns a list of all persons.</returns>
    [HttpGet("person")]
    public async Task<ActionResult<IEnumerable<Person>>> GetPersonsList() => Ok(await _service.GetPersonsListAsync().ConfigureAwait(false));

    /// <summary>
    /// Gets a specific person by their ID.
    /// </summary>
    /// <param name="id">The GUID of the person.</param>
    /// <returns>Returns the person if found, otherwise an error message.</returns>
    [HttpGet("person/{id}")]
    public async Task<ActionResult<Person>> GetPerson(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("ID is empty!");

            if (!Guid.TryParse(id, out Guid realEstatePersonID))
                return BadRequest("ID must be a valid GUID!");

            Person? person = await _service.GetPersonAsync(realEstatePersonID).ConfigureAwait(false);

            if (person is null)
                return NotFound("No such person found!");

            return person;
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
    }

    /// <summary>
    /// Adds a new person.
    /// </summary>
    /// <param name="newPerson">The person object to add.</param>
    /// <returns>Returns the created person with its location.</returns>
    [HttpPost("person/add")]
    public async Task<IActionResult> AddPerson([FromBody] Person newPerson)
    {
        if (newPerson == null)
            return BadRequest("Failed to retreive parameter!");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        bool? isPersonExists = await _service.IsPersonExistAsync(newPerson.PersonID).ConfigureAwait(false);

        if (isPersonExists == true)
            return BadRequest("Person is already exist!");

        var addedPerson = await _service.AddPersonAsync(newPerson).ConfigureAwait(false);

        return CreatedAtAction(nameof(GetPerson), new { id = addedPerson.Id }, addedPerson);
    }

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="updatePerson">The person object with updated values.</param>
    /// <returns>Returns the updated person if successful.</returns>
    [HttpPut("person/update")]
    public async Task<IActionResult> UpdatePerson([FromBody] Person updatePerson)
    {
        if (updatePerson == null)
            return BadRequest("Failed to retreive parameter!");

        Person? existPerson = await _service.GetPersonAsync(updatePerson.Id).ConfigureAwait(false);

        if (existPerson is null)
            return NotFound("No such person found!");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _service.UpdatePersonAsync(updatePerson).ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific person by their ID.
    /// </summary>
    /// <param name="id">The GUID of the person to delete.</param>
    /// <returns>Returns a success message if deleted, or an error message if not found or invalid.</returns>
    [HttpDelete("person/delete/{id}")]
    public async Task<IActionResult> DeletePerson(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("ID is empty!");

            if (!Guid.TryParse(id, out Guid personID))
                return BadRequest("ID must be a valid GUID!");

            Person? person = await _service.GetPersonAsync(personID).ConfigureAwait(false);

            if (person is null)
                return NotFound("No such person found!");

            await _service.DeletePersonAsync(person).ConfigureAwait(false);

            return Ok("Person successfully deleted");
        }
        catch (IOException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Unexpected format error.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(403, "Access denied.");
        }
    }

    /// <summary>
    /// Deletes all persons.
    /// </summary>
    /// <returns>Returns a success message after deleting all persons.</returns>
    [HttpDelete("person/delete-all")]
    public async Task<IActionResult> DeleteAllPersons()
    {
        await _service.DeleteAllPersonsAsync().ConfigureAwait(false);
        return Ok("All persons has been deleted!");
    }
    #endregion
}


