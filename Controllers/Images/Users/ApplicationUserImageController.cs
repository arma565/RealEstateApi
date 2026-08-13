using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RealEstate.DTOs.Images.Users;
using RealEstate.Entities.Images.Users;
using RealEstate.Services.Images.Users;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Images.Users;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class ApplicationUserImageController(ApplicationUserImageService service, ILogger<ApplicationUserImageController> logger) : ControllerBase
{
    private readonly ApplicationUserImageService _service = service;

    private readonly ILogger<ApplicationUserImageController> _logger = logger;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] CreateDTO applicationUserImageDTO, [FromForm] IFormFile[] images)
    {
        try
        {
            if (images == null || images.Length == 0)
                return BadRequest("No images provided for upload.");

            foreach (var image in images)
            {
                await _service.AddAsync(applicationUserImageDTO, image).ConfigureAwait(false);
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

    [HttpGet("download/{applicationUserImageId}")]
    public async Task<IActionResult> Download(string applicationUserImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(applicationUserImageId))
                return BadRequest("ApplicationUserImageId is empty!");

            if (!Guid.TryParse(applicationUserImageId, out Guid id))
                return BadRequest("ApplicationUserImageId must be a valid GUID!");

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

    [HttpGet("get/{applicationUserImageId}")]
    public async Task<ActionResult<ApplicationUserImage>> Get(string applicationUserImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(applicationUserImageId))
                return BadRequest("ApplicationUserImageId is empty!");

            if (!Guid.TryParse(applicationUserImageId, out Guid id))
                return BadRequest("ApplicationUserImageId must be a valid GUID!");

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

    [HttpPut("update/{applicationUserImageId}")]
    public async Task<ActionResult> Update(string applicationUserImageId, [FromBody] UpdateDTO applicationUserImageDTO, [FromForm] IFormFile[] images)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(applicationUserImageId))
                return BadRequest("ApplicationUserImageId is empty!");

            if (!Guid.TryParse(applicationUserImageId, out Guid id))
                return BadRequest("ApplicationUserImageId must be a valid GUID!");

            if (applicationUserImageDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (images == null || images.Length == 0)
                return BadRequest("No images provided for upload.");

            foreach (var image in images)
            {
                await _service.UpdateAsync(id, applicationUserImageDTO, image).ConfigureAwait(false);
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

    [HttpDelete("delete/{applicationUserImageId}")]
    public async Task<IActionResult> Delete(string applicationUserImageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(applicationUserImageId))
                return BadRequest("ApplicationUserImageId is empty!");

            if (!Guid.TryParse(applicationUserImageId, out Guid id))
                return BadRequest("ApplicationUserImageId must be a valid GUID!");

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
