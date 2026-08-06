using Microsoft.AspNetCore.Mvc;
using RealEstate.Entities.Properties.Addresses.Map;
using RealEstate.Repositories.Properties.Addresses.Maps;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Addresses.Maps;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class LocationController(LocationRepository service, ILogger<LocationController> logger) : ControllerBase
{
    private readonly LocationRepository _service = service;

    private readonly ILogger<LocationController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<PropertyLocation>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{locationId}")]
    public async Task<ActionResult<PropertyLocation>> Get(string locationId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(locationId))
                return BadRequest("LocationId is empty!");

            if (!Guid.TryParse(locationId, out Guid realEstateLocationId))
                return BadRequest("LocationId must be a valid GUID!");

            var location = await _service.GetAsync(realEstateLocationId).ConfigureAwait(false);

            return location == null ? NotFound() : Ok(location);
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
    public async Task<IActionResult> Add([FromBody] PropertyLocationDTO propertyLocationDTO)
    {
        try
        {
            if (propertyLocationDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

          var location =  await _service.AddAsync(propertyLocationDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { locationId = location.Id }, location);
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

    [HttpPut("update/{locationId}")]
    public async Task<IActionResult> Update([FromBody] PropertyLocationDTO propertyLocationDTO, string locationId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(locationId))
                return BadRequest("LocationId is empty!");

            if (!Guid.TryParse(locationId, out Guid realEstateLocationId))
                return BadRequest("LocationId must be a valid GUID!");

            if (propertyLocationDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(propertyLocationDTO , realEstateLocationId).ConfigureAwait(false);

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

    [HttpDelete("delete/{locationId}")]
    public async Task<IActionResult> Delete(string locationId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(locationId))
                return BadRequest("LocationId is empty!");

            if (!Guid.TryParse(locationId, out Guid realEstateLocationId))
                return BadRequest("LocationId must be a valid GUID!");

            await _service.DeleteAsync(realEstateLocationId).ConfigureAwait(false);

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
