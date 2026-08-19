using Microsoft.AspNetCore.Mvc;
using RealEstate.DTOs.Persons;
using RealEstate.Entities.Persons.Tenants;
using RealEstate.Services.Persons;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Persons;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class TenantController(TenantService service, ILogger<TenantController> logger) : ControllerBase
{
    private readonly TenantService _service = service;

    private readonly ILogger<TenantController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<Tenant>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{tenantId}")]
    public async Task<ActionResult<Tenant>> Get(string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId is empty!");

            if (!Guid.TryParse(tenantId, out Guid realEstateTenantid))
                return BadRequest("TenantId must be a valid GUID!");

            var tenant = await _service.GetAsync(realEstateTenantid).ConfigureAwait(false);

            return tenant == null ? NotFound() : Ok(tenant);
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

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] CreateDTO tenantDTO)
    {
        try
        {
            if (tenantDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

          var tenant =  await _service.AddAsync(tenantDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { tenantId = tenant.Id }, tenant);
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

    [HttpPut("update/{tenantId}")]
    public async Task<IActionResult> Update(string tenantId, [FromBody] UpdateDTO tenantDTO)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId is empty!");

            if (!Guid.TryParse(tenantId, out Guid id))
                return BadRequest("TenantId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(tenantDTO , id).ConfigureAwait(false);

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

    [HttpDelete("delete/{tenantId}")]
    public async Task<IActionResult> Delete(string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId is empty!");

            if (!Guid.TryParse(tenantId, out Guid id))
                return BadRequest("TenantId must be a valid GUID!");

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
