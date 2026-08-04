using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Properties.Addresses.Map;
using RealEstate.Services.Repositories.Properties.Addresses.Maps;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Addresses.Maps;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class LocationController(LocationRepository service, ILogger logger) : ControllerBase
{
    private readonly LocationRepository _service = service;

    private readonly ILogger _logger = logger;

    [HttpGet("/")]
    public async Task<IEnumerable<PropertyLocation>> GetListAsync() => [.. await _service.GetListAsync().ConfigureAwait(false)];

    [HttpGet("/{locationId}")]
    public async Task<ActionResult<PropertyLocation>> GetAsync(string locationId)
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
    public async Task<IActionResult> AddAsync([FromBody] PropertyLocationDTO propertyLocationDTO)
    {
        try
        {
            if (propertyLocationDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var location = new PropertyLocation
            {
                Latitude = propertyLocationDTO.Latitude,
                Longitude = propertyLocationDTO.Longitude,
                PropertyId = propertyLocationDTO.PropertyId,
            };

            await _service.AddAsync(location).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { locationId = location.Id }, location);
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
    public async Task<IActionResult> UpdateAsync([FromBody] PropertyLocationDTO propertyLocationDTO, string locationId)
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

            var existingLocation = await _service.IsLocationExistAsync(realEstateLocationId).ConfigureAwait(false);

            if (!existingLocation)
                return NotFound("Location not found!");

            var location = new PropertyLocation
            {
                Id = realEstateLocationId,
                Latitude = propertyLocationDTO.Latitude,
                Longitude = propertyLocationDTO.Longitude,
                PropertyId = propertyLocationDTO.PropertyId
            };

            await _service.UpdateAsync(location).ConfigureAwait(false);

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
    public async Task<IActionResult> DeleteAsync(string locationId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(locationId))
                return BadRequest("LocationId is empty!");

            if (!Guid.TryParse(locationId, out Guid realEstateLocationId))
                return BadRequest("LocationId must be a valid GUID!");

            var existingLocation = await _service.IsLocationExistAsync(realEstateLocationId).ConfigureAwait(false);

            if (!existingLocation)
                return NotFound("Location not found!");

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
}
