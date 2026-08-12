using Microsoft.AspNetCore.Mvc;
using RealEstate.DTOs.Properties.Leases;
using RealEstate.Entities.Properties.Leases;
using RealEstate.Repositories.Properties.Leases;
using RealEstate.Services.Properties.Leases;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Leases;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class LeaseController(LeaseService service,ILogger<LeaseController> logger) : ControllerBase
{
    private readonly LeaseService _service = service;

    private readonly ILogger<LeaseController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<Lease>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{leaseId}")]
    public async Task<ActionResult<Lease>> Get(string leaseId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(leaseId))
                return BadRequest("LeaseId is empty!");

            if (!Guid.TryParse(leaseId, out Guid id))
                return BadRequest("LeaseId must be a valid GUID!");

            var lease = await _service.GetAsync(id).ConfigureAwait(false);

            return lease == null ? NotFound() : Ok(lease);
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
    public async Task<IActionResult> Add([FromBody] LeaseDTO leaseDTO)
    {
        try
        {
            if (leaseDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lease = await _service.AddAsync(leaseDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { leaseId = lease.Id }, lease);
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

    [HttpPut("update/{leaseId}")]
    public async Task<IActionResult> Update(string leaseId, [FromBody] LeaseDTO leaseDTO)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(leaseId))
                return BadRequest("LeaseId is empty!");

            if (!Guid.TryParse(leaseId, out Guid id))
                return BadRequest("LeaseId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id,leaseDTO).ConfigureAwait(false);

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

    [HttpDelete("delete/{leaseId}")]
    public async Task<IActionResult> Delete(string leaseId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(leaseId))
                return BadRequest("LeaseId is empty!");

            if (!Guid.TryParse(leaseId, out Guid id))
                return BadRequest("LeaseId must be a valid GUID!");

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
