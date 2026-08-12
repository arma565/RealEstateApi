using Microsoft.AspNetCore.Mvc;
using RealEstate.DTOs.Properties;
using RealEstate.Entities.Properties;
using RealEstate.Services.Properties;
using RealEstate.Services.Validations;
using System.Security;


namespace RealEstate.Controllers.Properties;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public sealed class PropertyController(PropertyService service, ILogger<PropertyController> logger) : ControllerBase
{
    private readonly PropertyService _service = service;

    private readonly ILogger<PropertyController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<RealEstateProperty>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{propertyId}")]
    public async Task<ActionResult<RealEstateProperty>> Get(string propertyId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyId))
                return BadRequest("PropertyId is empty!");

            if (!Guid.TryParse(propertyId, out Guid id))
                return BadRequest("PropertyId must be a valid GUID!");

            var property = await _service.GetAsync(id).ConfigureAwait(false);

            return property == null ? NotFound() : Ok(property);

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
    public async Task<IActionResult> Add([FromBody] CreateDTO realEstatePropertyDTO)
    {
        try
        {
            if (realEstatePropertyDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           var realEstateProperty = await _service.AddAsync(realEstatePropertyDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { propertyId = realEstateProperty.Id }, realEstateProperty);
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

    [HttpPut("update/{propertyId}")]
    public async Task<IActionResult> Update(string propertyId, [FromBody] UpdateDTO realEstatePropertyDTO)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyId))
                return BadRequest("PropertyId is empty!");

            if (!Guid.TryParse(propertyId, out Guid id))
                return BadRequest("PropertyId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id,realEstatePropertyDTO).ConfigureAwait(false);

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

    [HttpDelete("delete/{propertyId}")]
    public async Task<IActionResult> Delete(string propertyId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(propertyId))
                return BadRequest("PropertyId is empty!");

            if (!Guid.TryParse(propertyId, out Guid id))
                return BadRequest("PropertyId must be a valid GUID!");

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
    public async Task<IActionResult> DeleteAllAssets()
    {
        try
        {
            await _service.DeleteAllAsync().ConfigureAwait(false);
            return Ok("All properties has been deleted!");
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


