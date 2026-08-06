using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RealEstate.Entities.Images.Properties;
using RealEstate.Services.Images.Properties;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Images.Properties;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class PropertyImageController(PropertyImageService service, ILogger<PropertyImageController> logger) : ControllerBase
{
    private readonly PropertyImageService _service = service;

    private readonly ILogger<PropertyImageController> _logger = logger;

    [HttpPost("upload/{propertyImageId}")]
    public async Task<IActionResult> Upload(string propertyImageId, [FromBody] PropertyImageDTO propertyImageDTO ,[FromForm] IFormFile[] images)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyImageId))
                return BadRequest("PropertyImageId is empty!");

            if (!Guid.TryParse(propertyImageId, out Guid id))
                return BadRequest("PropertyImageId must be a valid GUID!");

            if (images == null || images.Length == 0)
                return BadRequest("No images provided for upload.");

            foreach (var image in images)
            {
                await _service.UpdateAsync(id, propertyImageDTO ,image).ConfigureAwait(false);
            }

            return Ok("Images uploaded successfully");
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

    [HttpGet("download/{propertyImageId}")]
    public async Task<IActionResult> Download(string propertyImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyImageId))
                return BadRequest("PropertyImageId is empty!");

            if (!Guid.TryParse(propertyImageId, out Guid id))
                return BadRequest("PropertyImageId must be a valid GUID!");

            var fullPath = await _service.GetPathAsync(id).ConfigureAwait(false);

            if (fullPath == null || fullPath.FullOriginalPath == null)
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

    [HttpGet("get/{propertyImageId}")]
    public async Task<ActionResult<PropertyImage>> Get(string propertyImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyImageId))
                return BadRequest("PropertyImageId is empty!");

            if (!Guid.TryParse(propertyImageId, out Guid id))
                return BadRequest("PropertyImageId must be a valid GUID!");

            var propertyImage = await _service.GetAsync(id).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(propertyImage);
            return propertyImage;
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
    public async Task<ActionResult> Add([FromBody] PropertyImageDTO propertyImageDTO)
    {
        try
        {
            if (propertyImageDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var propertyImage = await _service.AddAsync(propertyImageDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { id = propertyImage.Id }, propertyImage);
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

    [HttpDelete("delete/{propertyImageId}")]
    public async Task<IActionResult> Delete(string propertyImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyImageId))
                return BadRequest("PropertyImageId is empty!");

            if (!Guid.TryParse(propertyImageId, out Guid id))
                return BadRequest("PropertyImageId must be a valid GUID!");

            await _service.DeleteAsync(id).ConfigureAwait(false);

            return Ok("Property image successfully deleted!");
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
