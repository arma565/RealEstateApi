using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RealEstate.DTOs.Images.Documents;
using RealEstate.Entities.Images.Documents;
using RealEstate.Services.Images.Documents;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Images.Documents;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class PropertyDeedImageController(PropertyDeedImageService service, ILogger<PropertyDeedImageController> logger) : ControllerBase
{
    private readonly PropertyDeedImageService _service = service;

    private readonly ILogger<PropertyDeedImageController> _logger = logger;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] CreateDTO propertyDeedImageDTO, [FromForm] IFormFile[] images)
    {
        try
        {
            if (images == null || images.Length == 0)
                return BadRequest("No images provided for upload.");

            foreach (var image in images)
            {
                await _service.AddAsync(propertyDeedImageDTO, image).ConfigureAwait(false);
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

    [HttpGet("download/{propertyDeedImageId}")]
    public async Task<IActionResult> Download(string propertyDeedImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyDeedImageId))
                return BadRequest("propertyDeedImageId is empty!");

            if (!Guid.TryParse(propertyDeedImageId, out Guid id))
                return BadRequest("propertyDeedImageId must be a valid GUID!");

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

    [HttpGet("get/{propertyDeedImageId}")]
    public async Task<ActionResult<PropertyDeedImage>> Get(string propertyDeedImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyDeedImageId))
                return BadRequest("PropertyImageId is empty!");

            if (!Guid.TryParse(propertyDeedImageId, out Guid id))
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

    [HttpPut("update/{propertyDeedImageId}")]
    public async Task<ActionResult> Update(string propertyDeedImageId, [FromBody] UpdateDTO propertyDeedImageDTO, [FromForm] IFormFile[] images)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyDeedImageId))
                return BadRequest("propertyDeedImageId is empty!");

            if (!Guid.TryParse(propertyDeedImageId, out Guid id))
                return BadRequest("propertyDeedImageId must be a valid GUID!");

            if (propertyDeedImageDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (images == null || images.Length == 0)
                return BadRequest("No images provided for upload.");

            foreach (var image in images)
            {
                await _service.UpdateAsync(id, propertyDeedImageDTO, image).ConfigureAwait(false);
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

    [HttpDelete("delete/{propertyDeedImageId}")]
    public async Task<IActionResult> Delete(string propertyDeedImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyDeedImageId))
                return BadRequest("propertyDeedImageId is empty!");

            if (!Guid.TryParse(propertyDeedImageId, out Guid id))
                return BadRequest("propertyDeedImageId must be a valid GUID!");

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
