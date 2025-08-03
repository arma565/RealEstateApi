using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Models.Estate;
using RealEstate.Models.Estate.Assets;
using RealEstate.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security;

#pragma warning disable CA1515
namespace RealEstate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class EstateController(RepositoryService service, ImageService imageService) : ControllerBase
    {
        private readonly RepositoryService _service = service;
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

                var asset = await _service.GetAsset(realEstateAssetID).ConfigureAwait(false);

                if (asset == null)
                    return NotFound("No such asset found!");

                if (images == null || images.Length == 0)
                    return BadRequest("Image list cannot be empty!");

                var imageUrlList = await _imageService.UploadImages(images).ConfigureAwait(false);

                foreach (var img in imageUrlList)
                {
                    var assetImage = new AssetImage()
                    {
                        FileName = img,
                        AssetID = realEstateAssetID
                    };
                    await _service.AddAssetImage(assetImage).ConfigureAwait(false);
                }

                return Ok(new
                {
                    Message = "Images uploaded successfully",
                    ImageUrls = imageUrlList
                });
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
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

                List<AssetImage> assetImagesList = await _service.GetAssetImageList().ConfigureAwait(false);

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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
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
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssetListDescending() => Ok(await _service.GetAssetListDescending().ConfigureAwait(false));

        /// <summary>
        /// Gets the list of assets in ascending order.
        /// </summary>
        /// <returns>Returns a list of assets ordered ascendingly.</returns>
        [HttpGet("asset/asc")]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssetListAscending() => Ok(await _service.GetAssetListAscending().ConfigureAwait(false));

        /// <summary>
        /// Gets the list of assets ordered by date modified.
        /// </summary>
        /// <returns>Returns a list of assets ordered by date modified.</returns>
        [HttpGet("asset/date")]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssetListDateModified() => Ok(await _service.GetAssetListDateModified().ConfigureAwait(false));

        /// <summary>
        /// Gets a specific asset by its ID.
        /// </summary>
        /// <param name="assetID">The GUID of the asset.</param>
        /// <returns>Returns the asset if found.</returns>
        [HttpGet("asset/{assetID}")]
        public async Task<ActionResult<Asset>> GetAsset(string assetID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assetID))
                    return BadRequest("AssetID is empty!");

                if (!Guid.TryParse(assetID, out Guid realEstateAssetID))
                    return BadRequest("AssetID must be a valid GUID!");

                Asset? asset = await _service.GetAsset(realEstateAssetID).ConfigureAwait(false);

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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
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
        public async Task<IActionResult> AddAsset([FromBody] Asset newAsset)
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

                bool? isAssetExists = await _service.IsAssetExist(newAsset.PlatesNumber!).ConfigureAwait(false);

                if (isAssetExists == true)
                    return BadRequest("Asset is already exist!");

                var allAssets = await _service.GetAssetListAscending().ConfigureAwait(false);

                if (!allAssets.IsNullOrEmpty())
                {
                    var lastAssetOrderID = allAssets.Last().OrderID;
                    newAsset.OrderID = lastAssetOrderID + 1;
                }
                else newAsset.OrderID = 1;


                Asset? addedAsset = await _service.AddAsset(newAsset).ConfigureAwait(false);

                return CreatedAtAction(
                    nameof(GetAsset),
                    new { AssetID = addedAsset!.Id },
                    addedAsset
                );
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
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
        public async Task<IActionResult> UpdateAsset([FromBody] Asset updateAsset)
        {
            try
            {
                if (updateAsset == null)
                    return BadRequest("Failed to retrieve parameter!");

                Asset? asset = await _service.GetAsset(updateAsset.Id).ConfigureAwait(false);

                if (asset is null)
                    return NotFound("No such asset found!");

                updateAsset.Date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                updateAsset.Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _service.UpdateAsset(updateAsset).ConfigureAwait(false);

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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
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

                Asset? asset = await _service.GetAsset(realEstateID).ConfigureAwait(false);

                if (asset is null)
                    return NotFound("No such asset found!");

                _service.DeleteAsset(asset);

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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
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
        public IActionResult DeleteAssets()
        {
            _service.DeleteAllAssets();
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

                AssetImage? assetImage = await _service.GetAssetImage(realEstateAssetImageID).ConfigureAwait(false);

                if (assetImage is null)
                    return NotFound("No such assetImage found!");

                _service.DeleteAssetImage(assetImage);

                return Ok("AssetImage successfully deleted");
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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
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
        public async Task<ActionResult<IEnumerable<Person>>> GetPersonsList() => Ok(await _service.GetPersonsList().ConfigureAwait(false));

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

                Person? person = await _service.GetPerson(realEstatePersonID).ConfigureAwait(false);

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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
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

            bool? isPersonExists = await _service.IsPersonExist(newPerson.PersonID).ConfigureAwait(false);

            if (isPersonExists == true)
                return BadRequest("Person is already exist!");

            var addedPerson = await _service.AddPerson(newPerson).ConfigureAwait(false);

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

            Person? existPerson = await _service.GetPerson(updatePerson.Id).ConfigureAwait(false);

            if (existPerson is null)
                return NotFound("No such person found!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedPerson = await _service.UpdatePerson(updatePerson).ConfigureAwait(false);

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

                Person? person = await _service.GetPerson(personID).ConfigureAwait(false);

                if (person is null)
                    return NotFound("No such person found!");

                _service.DeletePerson(person);

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
            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is SecurityException)
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
        public IActionResult DeleteAllPersons()
        {
            _service.DeleteAllPersons();
            return Ok("All persons has been deleted!");
        }
        #endregion
    }
}

