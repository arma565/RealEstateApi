using Microsoft.AspNetCore.Mvc;
using RealEstate.DTOs.Persons;
using RealEstate.Entities.Persons.Owners;
using RealEstate.Services.Persons;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Persons;

#pragma warning disable CA1515
[ApiController]
[Route("[controller]")]
public class OwnerController(OwnerService service, ILogger<OwnerController> logger) : ControllerBase
{
    private readonly OwnerService _service = service;

    private readonly ILogger<OwnerController> _logger = logger;

    [HttpGet("get-list")]
    public async Task<ActionResult<IEnumerable<Owner>>> GetList() => Ok(await _service.GetListAsync().ConfigureAwait(false));

    [HttpGet("get/{ownerId}")]
    public async Task<ActionResult<Owner>> Get(string ownerId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                return BadRequest("OwnerId is empty!");

            if (!Guid.TryParse(ownerId, out Guid realEstateOwnerid))
                return BadRequest("OwnerId must be a valid GUID!");

            var owner = await _service.GetAsync(realEstateOwnerid).ConfigureAwait(false);

            return owner == null ? NotFound() : Ok(owner);
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
    public async Task<IActionResult> Add([FromBody] CreateDTO ownerDTO)
    {
        try
        {
            if (ownerDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

          var owner =  await _service.AddAsync(ownerDTO).ConfigureAwait(false);

            return CreatedAtAction(nameof(Get), new { ownerId = owner.Id }, owner);
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

    [HttpPut("update/{ownerId}")]
    public async Task<IActionResult> Update([FromBody] UpdateDTO ownerDTO, string ownerId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                return BadRequest("OwnerId is empty!");

            if (!Guid.TryParse(ownerId, out Guid id))
                return BadRequest("OwnerId must be a valid GUID!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(ownerDTO , id).ConfigureAwait(false);

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

    [HttpDelete("delete/{ownerId}")]
    public async Task<IActionResult> Delete(string ownerId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ownerId))
                return BadRequest("OwnerId is empty!");

            if (!Guid.TryParse(ownerId, out Guid id))
                return BadRequest("OwnerId must be a valid GUID!");

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
