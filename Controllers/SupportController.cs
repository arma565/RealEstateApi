#pragma warning disable CA1515
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RealEstate.Models.Authentication.Users;
using RealEstate.Models.Support;
using RealEstate.Services;
using System.Security;

namespace RealEstate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class SupportController(RepositoryService service, ImageService imageService) : ControllerBase
    {
        private readonly RepositoryService _service = service;
        private readonly ImageService _imageService = imageService;

        #region Support
        /// <summary>
        /// Upload a support image
        /// </summary>
        /// <param name="supportID">The ID of the support.</param>
        /// <param name="image">The image file to upload.</param>
        /// <returns>Returns 200 OK if successful, 400 BadRequest for invalid input, 404 NotFound if support not found, or 500/403 for errors.</returns>
        [HttpPost("support/upload/{supportID}")]
        public async Task<IActionResult> UploadSupportImage(string supportID, IFormFile image)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(supportID))
                    return BadRequest("SupportID is empty!");

                if (!Guid.TryParse(supportID, out Guid realEstateSupportID))
                    return BadRequest("SupportID must be a valid GUID!");

                var support = await _service.GetSupport(realEstateSupportID).ConfigureAwait(false);

                if (support == null)
                    return NotFound("No such support found!");

                if (image == null || image.Length == 0)
                    return BadRequest("Image cannot be empty!");

                var imageUrl = await _imageService.UploadSupportImage(image).ConfigureAwait(false);

                var supportImage = new SupportImage()
                {
                    SupportImageFileName = imageUrl,
                    SupportId = realEstateSupportID
                };
                await _service.AddSupportImage(supportImage).ConfigureAwait(false);

                return Ok(new
                {
                    Message = "Image uploaded successfully",
                    ImageUrl = imageUrl
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
        /// Download the support image
        /// </summary>
        /// <param name="imageFileName">The file name of the image to download.</param>
        /// <returns>Returns the image file if found, 404 NotFound if not found, 400 BadRequest for invalid input, or 500/403 for errors.</returns>
        [HttpGet("support/download/{imageFileName}")]
        public async Task<IActionResult> DownloadSupportImage(string imageFileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageFileName))
                    return BadRequest("Image file name is empty!");

                var supportImageList = await _service.GetSupportImageList().ConfigureAwait(false);

                var supportImage = supportImageList.FirstOrDefault(supportImg => supportImg.SupportImageFileName == imageFileName);

                if (supportImage == null)
                    return NotFound("No such image found!");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(supportImage.SupportImageFileName);
                if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                    return BadRequest("Unsupported image file type.");

                var environmentPath = _imageService.GetLocalImagesFullPath("support");

                var fullPath = Path.Combine(environmentPath, supportImage.SupportImageFileName);

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
        /// Retrieves all supports.
        /// </summary>
        /// <returns>Returns a list of all supports.</returns>
        [HttpGet("support")]
        public async Task<ActionResult<List<Support>>> GetAllSupports() => Ok(await _service.GetSupportList().ConfigureAwait(false));

        /// <summary>
        /// Retrieves a support by Id.
        /// </summary>
        /// <param name="supportID">The id of the support.</param>
        /// <returns>Returns the support if found, 404 NotFound if not found, or 400 BadRequest for invalid input.</returns>
        [HttpGet("support/{supportID}")]
        public async Task<ActionResult<Support>> GetSupport(string supportID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(supportID))
                    return BadRequest("SupportID is empty!");

                if (!Guid.TryParse(supportID, out Guid realEstateSupportID))
                    return BadRequest("SupportID must be a valid GUID!");

                var support = await _service.GetSupport(realEstateSupportID).ConfigureAwait(false);

                if (support == null)
                    return NotFound("No such support found!");

                return Ok(support);

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
        /// add support.
        /// </summary>
        /// <param name="support">New support to add.</param>
        /// <returns>Returns 201 Created if successful</returns>
        [HttpPost("support/add")]
        public async Task<IActionResult> AddSupport([FromBody] Support support)
        {
            try
            {
                if (support == null)
                    return BadRequest("Failed to retreive parameter!");

                var allSupports = await _service.GetSupportList().ConfigureAwait(false);

                Support addedSupport = await _service.AddSupport(support).ConfigureAwait(false);

                return CreatedAtAction(nameof(GetSupport), new { SupportID = addedSupport.Id}, addedSupport);
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
        /// Update support
        /// </summary>
        /// <param name="support">The support model to update</param>
        /// <returns>Returns 204 NoContent if successful</returns>
        [HttpPost("support/update")]
        public async Task<IActionResult> UpdateSupport([FromBody] Support support)
        {
            try
            {
                if (support == null) 
                    return BadRequest("Failed to retreive parameter!");

                await _service.UpdateSupport(support).ConfigureAwait(false);

                return NoContent();
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
        /// Delete a specific support by ID.
        /// </summary>
        /// <param name="supportID">The id of the support.</param>
        /// <returns>Returns 204 NoContent if successful, 404 NotFound if user not found.</returns>
        [HttpDelete("support/delete/{supportID}")]
        public async Task<IActionResult> DeleteSupport(string supportID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(supportID))
                    return BadRequest("SupportID is empty!");

                if (!Guid.TryParse(supportID, out Guid realEstateSupportID))
                    return BadRequest("SupportID must be a valid GUID!");

                var support = await _service.GetSupport(realEstateSupportID).ConfigureAwait(false);

                if (support == null)
                    return NotFound("No such support found!");

                _service.DeleteSupport(support);
                return NoContent();
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
        /// Deletes all supports.
        /// </summary>
        /// <returns>Returns 204 NoContent if successful</returns>
        [HttpDelete("support/delete-all")]
        public async Task<IActionResult> DeleteAllSupports()
        {
            try
            {
                _service.DeleteAllSupports();
                await _service.GetSupportList().ConfigureAwait(false);
                return NoContent();
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

        #region SupportImage
        /// <summary>
        /// Deletes a support image by ID.
        /// </summary>
        /// <param name="supportImageID">The GUID of the support image to delete.</param>
        /// <returns>
        /// Returns 204 NoContent if the image was successfully deleted,
        /// 400 BadRequest if the ID is invalid,
        /// 404 NotFound if the image does not exist,
        /// or 500/403 for errors.
        /// </returns>
        [HttpDelete("support-image/delete/{supportImageID}")]
        public async Task<IActionResult> DeleteProfileImage(string supportImageID)
        {
            try
            {
                if (!Guid.TryParse(supportImageID, out Guid realEstateSupportImageID))
                    return BadRequest("SupportImageID must be a valid GUID!");

                SupportImage? supportImage = await _service.GetSupportImage(realEstateSupportImageID).ConfigureAwait(false);

                if (supportImage is null)
                    return NotFound("Image not found!");

                _service.DeleteSupportImage(supportImage);

                return NoContent();
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
    }
}
