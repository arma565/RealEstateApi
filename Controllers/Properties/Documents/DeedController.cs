using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Properties.Documents;
using RealEstate.Services.Repositories.Properties.Documents;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Documents;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class DeedController(DeedRepository service, ILogger<DeedController> logger) : ControllerBase
{
    private readonly DeedRepository _service = service;

    private readonly ILogger<DeedController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<PropertyDeed>>> GetListAsync() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{propertyDeedId}")]
    public async Task<ActionResult<PropertyDeed>> GetAsync(string propertyDeedId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyDeedId))
                return BadRequest("propertyDeedId is empty!");

            if (!Guid.TryParse(propertyDeedId, out Guid id))
                return BadRequest("propertyDeedId must be a valid GUID!");

            var deed = await _service.GetAsync(id).ConfigureAwait(false);

            return deed == null ? NotFound() : Ok(deed);
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

    [HttpPost("add")]
    public async Task<IActionResult> AddAsync([FromBody] PropertyDeedDTO propertyDeedDTO)
    {
        try
        {
            if (propertyDeedDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           var propertyDeed = await _service.AddAsync(propertyDeedDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { id = propertyDeed.Id }, propertyDeed);
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

    [HttpPut("update/{propertyDeedId}")]
    public async Task<IActionResult> UpdateAsync(string propertyDeedId, [FromBody] PropertyDeedDTO propertyDeedDTO)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyDeedId))
                return BadRequest("propertyDeedId is empty!");

            if (!Guid.TryParse(propertyDeedId, out Guid id))
                return BadRequest("propertyDeedId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id,propertyDeedDTO).ConfigureAwait(false);

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

    [HttpDelete("delete/{propertyDeedId}")]
    public async Task<IActionResult> DeleteAsync(string propertyDeedId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyDeedId))
                return BadRequest("DeedId is empty!");

            if (!Guid.TryParse(propertyDeedId, out Guid id))
                return BadRequest("DeedId must be a valid GUID!");

            await _service.DeleteAsync(id).ConfigureAwait(false);

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

    [HttpDelete("delete-all")]
    public async Task<IActionResult> DeleteAllAsync()
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
