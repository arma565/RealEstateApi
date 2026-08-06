using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RealEstate.Entities.Images;
using RealEstate.Repositories.Images;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Images;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class ImageController(ImageRepository service, ILogger<ImageController> logger) : ControllerBase
{
    private readonly ImageRepository _service = service;

    private readonly ILogger<ImageController> _logger = logger;

    [HttpPost("upload/{imageId}")]
    public async Task<IActionResult> Upload(string imageId, RealEstateImageDTO realEstateImageDTO ,IFormFile image)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageId))
                return BadRequest("ImageId is empty!");

            if (!Guid.TryParse(imageId, out Guid id))
                return BadRequest("ImageId must be a valid GUID!");

            await _service.UpdateAsync(id , realEstateImageDTO, image).ConfigureAwait(false);

            return Ok("Image uploaded successfully");
        }
        catch (IOException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpGet("download/{imageId}")]
    public async Task<IActionResult> Download(string imageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageId))
                return BadRequest("ImageId is empty!");

            if (!Guid.TryParse(imageId, out Guid id))
                return BadRequest("ImageId must be a valid GUID!");

            var fullPath = await _service.GetPathAsync(id).ConfigureAwait(false);

            if(fullPath == null || fullPath.FullOriginalPath == null)
                return NotFound("Image not found!");

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fullPath.FullOriginalPath, out var contentType))
                contentType = "application/octet-stream";

            return PhysicalFile(fullPath.FullOriginalPath, contentType, Path.GetFileName(fullPath.FullOriginalPath));
        }
        catch (IOException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "File system error occurred while uploading images.");
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpGet("get/{imageId}")]
    public async Task<ActionResult<RealEstateImage>> Get(string imageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageId))
                return BadRequest("ImageId is empty!");

            if (!Guid.TryParse(imageId, out Guid realEstateImageid))
                return BadRequest("ImageId must be a valid GUID!");

            var realEstateImage = await _service.GetAsync(realEstateImageid).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(realEstateImage);
            return realEstateImage;
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult> Add([FromBody] RealEstateImageDTO realEstateImageDTO)
    {
        try
        {
            if (realEstateImageDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var realEstateImage = await _service.AddAsync(realEstateImageDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { id = realEstateImage.Id }, realEstateImage);
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
    }

    [HttpDelete("delete/{imageId}")]
    public async Task<IActionResult> Delete(string imageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageId))
                return BadRequest("ImageId is empty!");

            if (!Guid.TryParse(imageId, out Guid realEstateImageid))
                return BadRequest("ImageId must be a valid GUID!");

            await _service.DeleteAsync(realEstateImageid).ConfigureAwait(false);

            return Ok("Image successfully deleted!");
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }

    }

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAll()
    {
        try
        {
            await _service.DeleteAllAsync().ConfigureAwait(false);
            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "Missing argument. Please contact support.");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "An invalid operation occurred.");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied.");
        }

    }
}
