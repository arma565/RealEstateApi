using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RealEstate.DTOs.Images.Properties;
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

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] CreateDTO propertyImageDTO, [FromForm] IFormFile[] images)
    {
        try
        {
            if (images == null || images.Length == 0)
                return BadRequest("No images provided for upload.");

            foreach (var image in images)
            {
                await _service.AddAsync(propertyImageDTO, image).ConfigureAwait(false);
            }

            return Created();
        }
        catch (IOException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "File system error occurred while uploading images!");
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "Required argument is missing!");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "An invalid operation occurred!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
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
            return StatusCode(500, "File system error occurred while uploading images!");
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "Required argument is missing!");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "An invalid operation occurred!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
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

            return await _service.GetAsync(id).ConfigureAwait(false); ;
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
    }

    [HttpPut("update/{propertyImageId}")]
    public async Task<ActionResult> Update(string propertyImageId, [FromBody] UpdateDTO propertyImageDTO, [FromForm] IFormFile[] images)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyImageId))
                return BadRequest("PropertyImageId is empty!");

            if (!Guid.TryParse(propertyImageId, out Guid id))
                return BadRequest("PropertyImageId must be a valid GUID!");

            if (propertyImageDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (images == null || images.Length == 0)
                return BadRequest("No images provided for upload.");

            foreach (var image in images)
            {
                await _service.UpdateAsync(id, propertyImageDTO, image).ConfigureAwait(false);
            }

            return NoContent();
        }
        catch (IOException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(500, "File system error occurred while uploading images!");
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "Required argument is missing!");
        }
        catch (InvalidOperationException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "An invalid operation occurred!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
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

            return NoContent();
        }
        catch (ArgumentNullException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
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
            return StatusCode(400, "Required argument is missing!");
        }
        catch (UnauthorizedAccessException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }
        catch (SecurityException ex)
        {
            LogMessages.UnexpectedError(_logger, ex);
            return StatusCode(403, "Access denied!");
        }

    }
}
