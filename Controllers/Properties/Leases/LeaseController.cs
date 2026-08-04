using Microsoft.AspNetCore.Mvc;
using RealEstate.Services.Models.Properties.Leases;
using RealEstate.Services.Repositories.Properties.Leases;
using RealEstate.Services.Validations;
using System.Security;

namespace RealEstate.Controllers.Properties.Leases;

#pragma warning disable CA1515
[Route("[controller]")]
[ApiController]
public class LeaseController(LeaseRepository service,ILogger logger) : ControllerBase
{
    private readonly LeaseRepository _service = service;

    private readonly ILogger _logger = logger;

    [HttpGet("/")]
    public async Task<IEnumerable<Lease>> GetListAsync() => [.. await _service.GetListAsync().ConfigureAwait(false)];

    [HttpGet("/{leaseId}")]
    public async Task<ActionResult<Lease>> GetAsync(string leaseId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(leaseId))
                return BadRequest("LeaseId is empty!");

            if (!Guid.TryParse(leaseId, out Guid realEstateLeaseId))
                return BadRequest("LeaseId must be a valid GUID!");

            var lease = await _service.GetAsync(realEstateLeaseId).ConfigureAwait(false);

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
    public async Task<IActionResult> AddAsync([FromBody] LeaseDTO leaseDTO)
    {
        try
        {
            if (leaseDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lease = new Lease
            {
                MonthlyRent = leaseDTO.MonthlyRent,
                DepositAmount = leaseDTO.DepositAmount,
                StartTime = leaseDTO.StartTime ?? TimeOnly.MinValue,
                EndTime = leaseDTO.EndTime,
                StartDate = leaseDTO.StartDate,
                EndDate = leaseDTO.EndDate,
                PropertyId = leaseDTO.PropertyId
            };

            await _service.AddAsync(lease).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetAsync), new { leaseId = lease.Id }, lease);
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
    public async Task<IActionResult> UpdateAsync([FromBody] LeaseDTO leaseDTO, string leaseId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(leaseId))
                return BadRequest("LeaseId is empty!");

            if (!Guid.TryParse(leaseId, out Guid realEstateLeaseId))
                return BadRequest("LeaseId must be a valid GUID!");

            if (leaseDTO == null)
                return BadRequest("Failed to retrieve parameter!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingLease = await _service.IsLeaseExistAsync(realEstateLeaseId).ConfigureAwait(false);

            if (!existingLease)
                return NotFound("Lease not found!");

            var lease = new Lease
            {
                Id = realEstateLeaseId,
                MonthlyRent = leaseDTO.MonthlyRent,
                DepositAmount = leaseDTO.DepositAmount,
                StartTime = leaseDTO.StartTime ?? TimeOnly.MinValue,
                EndTime = leaseDTO.EndTime,
                StartDate = leaseDTO.StartDate,
                EndDate = leaseDTO.EndDate,
                PropertyId = leaseDTO.PropertyId
            };

            await _service.UpdateAsync(lease).ConfigureAwait(false);

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
    public async Task<IActionResult> DeleteAsync(string leaseId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(leaseId))
                return BadRequest("LeaseId is empty!");

            if (!Guid.TryParse(leaseId, out Guid realEstateLeaseId))
                return BadRequest("LeaseId must be a valid GUID!");

            var existingLease = await _service.IsLeaseExistAsync(realEstateLeaseId).ConfigureAwait(false);

            if (!existingLease)
                return NotFound("Lease not found!");

            await _service.DeleteAsync(realEstateLeaseId).ConfigureAwait(false);

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
