using Microsoft.AspNetCore.Mvc;
using RealEstate.Entities.Properties.Features;
using RealEstate.Repositories.Properties.Features;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Features;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class FeatureController(FeatureRepository service, ILogger<FeatureController> logger) : ControllerBase
{
    private readonly FeatureRepository _service = service;

    private readonly ILogger<FeatureController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<PropertyFeature>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{featureId}")]
    public async Task<ActionResult<PropertyFeature>> Get(string featureId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(featureId))
                return BadRequest("FeatureId is empty!");

            if (!Guid.TryParse(featureId, out Guid id))
                return BadRequest("FeatureId must be a valid GUID!");

            var feature = await _service.GetAsync(id).ConfigureAwait(false);

            return feature == null ? NotFound() : Ok(feature);
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
    public async Task<IActionResult> Add([FromBody] PropertyFeatureDTO propertyFeatureDTO)
    {
        try
        {
            if (propertyFeatureDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           var feature = await _service.AddAsync(propertyFeatureDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { featureId = feature.Id }, feature);
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

    [HttpPut("update/{featureId}")]
    public async Task<IActionResult> Update([FromBody] PropertyFeatureDTO propertyFeatureDTO, string featureId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(featureId))
                return BadRequest("FeatureId is empty!");

            if (!Guid.TryParse(featureId, out Guid id))
                return BadRequest("FeatureId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id,propertyFeatureDTO).ConfigureAwait(false);

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

    [HttpDelete("delete/{featureId}")]
    public async Task<IActionResult> Delete(string featureId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(featureId))
                return BadRequest("FeatureId is empty!");

            if (!Guid.TryParse(featureId, out Guid id))
                return BadRequest("FeatureId must be a valid GUID!");

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
