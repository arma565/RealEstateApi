using Microsoft.AspNetCore.Mvc;
using RealEstate.DTOs.Supports;
using RealEstate.Entities.Supports;
using RealEstate.Repositories.Supports;
using RealEstate.Services.Supports;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Supports;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public sealed class SupportController(SupportService service, ILogger<SupportController> logger) : ControllerBase
{
    private readonly SupportService _service = service;

    private readonly ILogger<SupportController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<RealEstateSupport>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{supportId}")]
    public async Task<ActionResult<RealEstateSupport>> Get(string supportId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportId))
                return BadRequest("SupportId is empty!");

            if (!Guid.TryParse(supportId, out Guid id))
                return BadRequest("SupportId must be a valid GUID!");

            var support = await _service.GetAsync(id).ConfigureAwait(false);

            return support == null ? NotFound() : Ok(support);

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
    public async Task<IActionResult> Add([FromBody] RealEstateSupportDTO realEstateSupportDTO)
    {
        try
        {
            if (realEstateSupportDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var realEstateSupport = await _service.AddAsync(realEstateSupportDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { supportId = realEstateSupport.Id }, realEstateSupport);
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

    [HttpPost("update/{supportId}")]
    public async Task<IActionResult> Update(string supportId, [FromBody] RealEstateSupportDTO realEstateSupportDTO)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportId))
                return BadRequest("SupportId is empty!");

            if (!Guid.TryParse(supportId, out Guid id))
                return BadRequest("SupportId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id,realEstateSupportDTO).ConfigureAwait(false);

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

    [HttpDelete("delete/{supportId}")]
    public async Task<IActionResult> Delete(string supportId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(supportId))
                return BadRequest("SupportId is empty!");

            if (!Guid.TryParse(supportId, out Guid id))
                return BadRequest("SupportId must be a valid GUID!");

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
    public async Task<IActionResult> DeleteAllSupports()
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

