using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Images;
using RealEstate.Services.Repositories.Images;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Images;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class ImageController(ImageRepository service, ILogger logger) : ControllerBase
{
    private readonly ImageRepository _service = service;

    private readonly ILogger _logger = logger;

    [HttpPost("upload/{imageId}")]
    public async Task<IActionResult> UploadAsync([FromForm] IFormFile image, string imageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageId))
                return BadRequest("ImageId is empty!");

            if (!Guid.TryParse(imageId, out Guid realEstateImageid))
                return BadRequest("ImageId must be a valid GUID!");

            await _service.UploadAsync(image, realEstateImageid).ConfigureAwait(false);

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

    [HttpGet("download/{imageId}")]
    public async Task<IActionResult> DownloadAsync(string imageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageId))
                return BadRequest("ImageId is empty!");

            if (!Guid.TryParse(imageId, out Guid realEstateImageid))
                return BadRequest("ImageId must be a valid GUID!");

            return Ok(await _service.DownloadAsync(realEstateImageid).ConfigureAwait(false));
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

    [HttpGet("{imageId}")]
    public async Task<ActionResult<RealEstateImage>> GetAsync(string imageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(imageId))
                return BadRequest("ImageId is empty!");

            if (!Guid.TryParse(imageId, out Guid realEstateImageid))
                return BadRequest("ImageId must be a valid GUID!");

            var realEstateImage = await _service.GetByIdAsync(realEstateImageid).ConfigureAwait(false);
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
    public async Task<ActionResult> AddAsync([FromBody] RealEstateImageDTO realEstateImageDTO)
    {
        try
        {
            if (realEstateImageDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var realEstateImage = await _service.AddAsync(realEstateImageDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { id = realEstateImage.Id }, realEstateImage);
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
    public async Task<IActionResult> DeleteAsync(string imageId)
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
}
