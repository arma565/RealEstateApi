using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RealEstate.DTOs.Images.Supports;
using RealEstate.Entities.Images.Supports;
using RealEstate.Services.Images.Supports;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Images.Supports;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class SupportImageController(SupportImageService service, ILogger<SupportImageController> logger) : ControllerBase
{
    private readonly SupportImageService _service = service;

    private readonly ILogger<SupportImageController> _logger = logger;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] CreateDTO supportImageDTO, [FromForm] IFormFile[] images)
    {
        try
        {
            if (images == null || images.Length == 0)
                return BadRequest("No images provided for upload.");

            foreach (var image in images)
            {
                await _service.AddAsync(supportImageDTO, image).ConfigureAwait(false);
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

    [HttpGet("download/{supportImageId}")]
    public async Task<IActionResult> Download(string supportImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportImageId))
                return BadRequest("SupportImageId is empty!");

            if (!Guid.TryParse(supportImageId, out Guid id))
                return BadRequest("SupportImageId must be a valid GUID!");

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

    [HttpGet("get/{supportImageId}")]
    public async Task<ActionResult<SupportImage>> Get(string supportImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportImageId))
                return BadRequest("SupportImageId is empty!");

            if (!Guid.TryParse(supportImageId, out Guid id))
                return BadRequest("SupportImageId must be a valid GUID!");

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

    [HttpPut("update/{supportImageId}")]
    public async Task<ActionResult> Update(string supportImageId, [FromBody] UpdateDTO supportImageDTO, [FromForm] IFormFile[] images)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportImageId))
                return BadRequest("SupportImageId is empty!");

            if (!Guid.TryParse(supportImageId, out Guid id))
                return BadRequest("SupportImageId must be a valid GUID!");

            if (supportImageDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (images == null || images.Length == 0)
                return BadRequest("No images provided for upload.");

            foreach (var image in images)
            {
                await _service.UpdateAsync(id, supportImageDTO, image).ConfigureAwait(false);
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

    [HttpDelete("delete/{supportImageId}")]
    public async Task<IActionResult> Delete(string supportImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportImageId))
                return BadRequest("SupportImageId is empty!");

            if (!Guid.TryParse(supportImageId, out Guid id))
                return BadRequest("SupportImageId must be a valid GUID!");

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
