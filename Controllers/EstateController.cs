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

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class EstateController(RepositoryService service, ImageService imageService) : ControllerBase
    {
        private readonly RepositoryService _service = service;
        private readonly ImageService _imageService = imageService;

        #region "Asset"
        [HttpPost("asset/upload/{assetID}")]
        public async Task<IActionResult> AssetImageUpload(string assetID, [FromForm] IFormFile[] images)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assetID))
                    return BadRequest("AssetID is empty!");

                if (!Guid.TryParse(assetID, out Guid realEstateAssetID))
                    return BadRequest("assetID must be a valid GUID!");

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
                        AssetID = realEstateAssetID,
                    };
                    await _service.AddAssetImage(assetImage).ConfigureAwait(false);
                }

                return Ok(new
                {
                    Message = "Images added successfully",
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

                // Detect MIME type
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
        [HttpGet("asset/desc")]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssetListDescending() => Ok(await _service.GetAssetListDescending().ConfigureAwait(false));
        [HttpGet("asset/asc")]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssetListAscending() => Ok(await _service.GetAssetListAscending().ConfigureAwait(false));
        [HttpGet("asset/date")]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssetListDateModified() => Ok(await _service.GetAssetListDateModified().ConfigureAwait(false));
        [HttpGet("asset/{assetID}")]
        public async Task<ActionResult<Asset>> GetAsset(string assetID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(assetID))
                    return BadRequest("Invalid assetID!");

                if (!Guid.TryParse(assetID, out Guid realEstateAssetID))
                    return BadRequest("assetID must be a valid GUID!");

                Asset? asset = await _service.GetAsset(realEstateAssetID).ConfigureAwait(false);

                if (asset is null)
                    return NotFound("Asset not found!");

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
                    return BadRequest("Asset with this plates number is already exist!");

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
        [HttpPut("asset/update")]
        public async Task<IActionResult> UpdateAsset([FromBody] Asset updateAsset)
        {
            try
            {
                if (updateAsset == null)
                    return BadRequest("Failed to retreive parameter!");

                Asset? asset = await _service.GetAsset(updateAsset.Id).ConfigureAwait(false);

                if (asset is null)
                    return NotFound("Asset not found!");

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
        [HttpDelete("asset/delete/{assetID}")]
        public async Task<IActionResult> DeleteAsset(string assetID)
        {
            try
            {
                if (!Guid.TryParse(assetID, out Guid realEstateID))
                    return BadRequest("Id must be a valid GUID!");

                Asset? asset = await _service.GetAsset(realEstateID).ConfigureAwait(false);

                if (asset is null)
                    return NotFound("Asset not found!");

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
        [HttpDelete("asset/delete-all")]
        public IActionResult DeleteAssets()
        {
            _service.DeleteAllAssets();
            return Ok("All assets has been deleted!");
        }
        #endregion

        #region AssetImage
        [HttpDelete("assetImage/delete/{assetImageID}")]
        public async Task<IActionResult> DeleteAssetImage(string assetImageID)
        {
            try
            {
                if (!Guid.TryParse(assetImageID, out Guid realEstateAssetImageID))
                    return BadRequest("Id must be a valid GUID!");

                AssetImage? assetImage = await _service.GetAssetImage(realEstateAssetImageID).ConfigureAwait(false);

                if (assetImage is null)
                    return NotFound("AssetImage not found!");

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

        #region "Person"
        [HttpGet("person")]
        public async Task<IEnumerable<Person>> GetPersonsList() => await _service.GetPersonsList().ConfigureAwait(false);
        [HttpGet("person/{id}")]
        public async Task<ActionResult<Person>> GetPerson(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("id is empty!");

                if (!Guid.TryParse(id, out Guid personID))
                    return BadRequest("id must be a valid GUID!");

                Person? person = await _service.GetPerson(personID).ConfigureAwait(false);

                if (person is null)
                    return NotFound("Person not found!");

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
        [HttpPost("person/add")]
        public async Task<IActionResult> AddPerson([FromBody] Person newPerson)
        {
            if (newPerson == null)
                return BadRequest("Failed to retreive parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existAsset = await _service.GetAsset(newPerson.AssetID).ConfigureAwait(false);

            if (existAsset is null)
                return NotFound("AssetID is incorrect or Asset not found!");

            var addedPerson = await _service.AddPerson(newPerson).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetPerson), new { id = addedPerson.Id }, addedPerson);
        }
        [HttpPut("person/update")]
        public async Task<IActionResult> UpdatePerson([FromBody] Person updatePerson)
        {
            if (updatePerson == null)
                return BadRequest("Failed to retreive parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existAsset = await _service.GetAsset(updatePerson.AssetID).ConfigureAwait(false);

            if (existAsset is null)
                return NotFound("AssetID is incorrect or Asset not found!");

            Person? existPerson = await _service.GetPerson(updatePerson.Id).ConfigureAwait(false);

            if (existPerson is null)
                return NotFound("Person not found!");

            var updatedPerson = await _service.UpdatePerson(updatePerson).ConfigureAwait(false);

            return Ok(updatedPerson);
        }
        [HttpDelete("person/delete/{id}")]
        public async Task<IActionResult> DeletePerson(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("id is empty!");

                if (!Guid.TryParse(id, out Guid personID))
                    return BadRequest("id must be a valid GUID!");

                Person? person = await _service.GetPerson(personID).ConfigureAwait(false);

                if (person is null)
                    return NotFound("Person not found!");

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
        [HttpDelete("person/delete-all")]
        public IActionResult DeleteAllPersons()
        {
            _service.DeleteAllPersons();
            return Ok("All persons has been deleted!");
        }
        #endregion
    }
}

