using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Models.Estate;
using RealEstate.Models.Estate.Assets;
using RealEstate.Services;
using System.Collections.ObjectModel;
using System.Globalization;

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
        public async Task<IActionResult> AssetImageUpload(Guid assetID, Collection<IFormFile> images)
        {
            try
            {
                var asset = await _service.GetAsset(assetID).ConfigureAwait(false);

                if (asset == null)
                    return NotFound("No such asset found");

                if (images.IsNullOrEmpty())
                    return BadRequest("Image list can not be empty!");

                var imageUrlList = await _imageService.UploadImages(images).ConfigureAwait(false);

                foreach (var img in imageUrlList)
                {
                    var assetImage = new AssetImage()
                    {
                        FileName = img,
                        AssetID = assetID
                    };
                    await _service.AddAssetImage(assetImage).ConfigureAwait(false);
                }

                return Ok("Images added successfully");
            }
            catch (IOException ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later." + "Error detailes: " + ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later." + "Error detailes: " + ex.Message);
            }
        }

        [HttpGet("asset/download/{imageFileName}")]
        public async Task<IActionResult> DownloadAssetImages(string imageFileName)
        {
            try
            {
                List<AssetImage> assetImagesList = await _service.GetAssetImagesList().ConfigureAwait(false);

                if (assetImagesList.IsNullOrEmpty())
                    return NotFound("list is empty!");

                var environmentPath = _imageService.GetLocalImagesFullPath("asset");

                var imgFileName = assetImagesList.FirstOrDefault(assetImg => assetImg.FileName == imageFileName);

                if (imgFileName == null)
                    return NotFound("No such image found!");

                var fullPath = Path.Combine(environmentPath + imgFileName.FileName);

                if (!System.IO.File.Exists(fullPath))
                    return NotFound("Image file not found!");

                // Detect MIME type
                var provider = new FileExtensionContentTypeProvider();

                if (!provider.TryGetContentType(fullPath, out var contentType))
                    contentType = "application/octet-stream";

                return PhysicalFile(fullPath, contentType, Path.GetFileName(fullPath));
            }
            catch (BadHttpRequestException ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("asset")]
        public async Task<IEnumerable<Asset>> GetAssetList() => await _service.GetAssetList().ConfigureAwait(false);

        [HttpGet("asset/{assetID}")]
        public async Task<ActionResult<Asset>> GetAsset(Guid assetID)
        {
            if (string.IsNullOrEmpty(assetID.ToString()))
                return BadRequest("Invalid assetID");

            Asset? asset = await _service.GetAsset(assetID).ConfigureAwait(false);

            if (asset is null)
                return NotFound("Asset not found!");

            return asset;
        }

        [HttpPost("asset/add")]
        public async Task<IActionResult> AddAsset([FromBody] Asset newAsset)
        {
            if (newAsset == null)
                return BadRequest("Failed to retreive parameter!");


            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if (newAsset.Date!.ToString().IsNullOrEmpty() && newAsset.Time.IsNullOrEmpty())
                newAsset.Date = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            newAsset.Time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);


            bool? isAssetExists = await _service.IsAssetExist(newAsset.PlatesNumber!).ConfigureAwait(false);

            if (isAssetExists == true)
                return BadRequest("Asset with this plates number is already exist!");

            Asset? addedAsset = await _service.AddAsset(newAsset).ConfigureAwait(false);

            return CreatedAtAction(
                nameof(GetAsset),
                new { AssetID = addedAsset!.Id },
                addedAsset
            );
        }

        [HttpPut("asset/update")]
        public async Task<IActionResult> UpdateAsset([FromBody] Asset updateAsset)
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

        [HttpDelete("asset/delete/{id}")]
        public async Task<IActionResult> DeleteAsset(Guid id)
        {
            Asset? asset = await _service.GetAsset(id).ConfigureAwait(false);

            if (asset is null)
                return NotFound("Asset not found!");

            _service.DeleteAsset(asset);

            return Ok("Asset successfully deleted");
        }

        [HttpDelete("asset/delete-all")]
        public IActionResult DeleteAssets()
        {
            _service.DeleteAllAssets();
            return Ok("All assets has been deleted!");
        }
        #endregion

        #region "Person"

        [HttpGet("persons")]
        public async Task<IEnumerable<Person>> GetPersonsList() => await _service.GetPersonsList().ConfigureAwait(false);

        [HttpGet("person/{id}")]
        public async Task<ActionResult<Person>> GetPerson(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Person? person = await _service.GetPerson(id).ConfigureAwait(false);

            if (person is null)
                return NotFound("Person not found!");

            return person;
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
        public async Task<IActionResult> DeletePerson(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Person? person = await _service.GetPerson(id).ConfigureAwait(false);

            if (person is null)
                return NotFound("Person not found!");

            _service.DeletePerson(person);

            return Ok("Person successfully deleted");
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

